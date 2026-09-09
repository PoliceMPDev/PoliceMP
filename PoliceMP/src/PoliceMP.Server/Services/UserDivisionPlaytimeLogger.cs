using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySql.Data.MySqlClient;

namespace PoliceMP.Server.Services
{
    public class UserDivisionPlaytimeLogger : BaseScript
    {
        private readonly Dictionary<string, UserDivisionSession> _activeSessions = new();

        private const string ConnectionString =
            "SERVER=pp870151-002.eu.clouddb.ovh.net;PORT=35658;DATABASE=pmp-dashboard-prod-913375;UID=srv-p-dash-35187;PASSWORD=frqfSVCzTBMHMgfv0lrA7wwgfyPx6B;";

        public UserDivisionPlaytimeLogger()
        {
            EventHandlers["UserDivisionPlaytime:StartSession"] +=
                new Func<Player, string, string, Task>(OnStartSession);
            EventHandlers["playerDropped"] += new Func<Player, string, Task>(OnPlayerDropped);
            EventHandlers["RolePlaytime:LogSession"] += new Action<Player, string>(OnLogSession);
            EventHandlers["RequestDiscordID"] += new Action<Player>(OnRequestDiscordId);
            EventHandlers["chatstatus:updateTyping"] += new Action<Player, bool>(OnTypingStatusChanged);

            Tick += CheckAfkStatus;

            Debug.WriteLine("[UserDivisionPlaytimeLogger] Started and listening.");
        }

        private void OnRequestDiscordId([FromSource] Player player)
        {
            var discordId = GetDiscordId(player);
            player.TriggerEvent("ReceiveDiscordID", discordId);
        }

        private string GetDiscordId(Player player)
        {
            int numIdentifiers = API.GetNumPlayerIdentifiers(player.Handle);

            for (int i = 0; i < numIdentifiers; i++)
            {
                var identifier = API.GetPlayerIdentifier(player.Handle, i);
                if (!string.IsNullOrWhiteSpace(identifier) && identifier.StartsWith("discord:"))
                    return identifier.Replace("discord:", "");
            }

            return "unknown";
        }

        private async Task OnStartSession([FromSource] Player player, string branch, string division)
        {
            await SaveSession(player);

            var discordId = GetDiscordId(player);
            if (discordId == "unknown") return;

            var ped = API.GetPlayerPed(player.Handle);
            var coords = API.GetEntityCoords(ped);

            _activeSessions[discordId] = new UserDivisionSession
            {
                Player = player,
                PlayerName = player.Name,
                DiscordId = discordId,
                Branch = branch,
                Division = division,
                StartTime = DateTime.UtcNow,
                LastPosition = new Vector3(coords.X, coords.Y, coords.Z),
                LastMoveTime = DateTime.UtcNow,
                AfkDuration = TimeSpan.Zero,
                IsAfk = false
            };

            Debug.WriteLine($"[UserDivision] {player.Name} started session in {branch} - {division}");
        }

        private async Task OnPlayerDropped([FromSource] Player player, string reason)
        {
            await SaveSession(player);

            var discordId = GetDiscordId(player);
            _activeSessions.Remove(discordId);
            _afkQueue.Remove(discordId);
        }

        private void OnLogSession([FromSource] Player player, string jsonData)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<LogSessionData>(jsonData);
            if (data == null) return;

            SaveLoggedSession(data.DiscordId, data.PlayerName, data.Month, data.Branch, data.Division,
                data.PlaytimeSeconds);
        }

        private async Task SaveSession(Player player)
        {
            var discordId = GetDiscordId(player);
            if (discordId == "unknown" || !_activeSessions.TryGetValue(discordId, out var session))
                return;

            _activeSessions.Remove(discordId);

            // Track AFK time if still AFK when player drops
            if (session.IsAfk)
            {
                session.AfkDuration += DateTime.UtcNow - session.LastMoveTime;
            }

            var duration = (DateTime.UtcNow - session.StartTime).TotalSeconds;
            var afkSeconds = session.AfkDuration.TotalSeconds;
            var netPlaytime = duration - afkSeconds;

            Debug.WriteLine(
                $"[UserDivision] {session.PlayerName} total: {(int)duration}s, AFK: {(int)afkSeconds}s, Net: {(int)netPlaytime}s");

            if (netPlaytime < 10)
                return;

            await SaveLoggedSession(session.DiscordId, session.PlayerName, DateTime.UtcNow.ToString("yyyy-MM"),
                session.Branch, session.Division, (int)netPlaytime);
        }

        private async Task SaveLoggedSession(string discordId, string playerName, string month, string branch,
            string division, int playtimeSeconds)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(discordId) || discordId == "unknown")
            {
                Debug.WriteLine("[UserDivision] Invalid discordId, skipping save.");
                return;
            }

            if (string.IsNullOrEmpty(playerName))
            {
                Debug.WriteLine("[UserDivision] Invalid playerName, skipping save.");
                return;
            }

            // Before saving → show saving info
            Debug.WriteLine(
                $"[UserDivision] Saving {playtimeSeconds} seconds for {playerName} - {branch} - {division}");

            await Task.Run(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(ConnectionString))
                    {
                        await connection.OpenAsync();

                        string columnName = GetDivisionColumn(branch, division);
                        if (string.IsNullOrEmpty(columnName))
                        {
                            Debug.WriteLine($"[UserDivision] No mapping for {branch} - {division}, skipping.");
                            return;
                        }

                        // Insert or ensure row exists
                        string insertQuery = @"
                    INSERT INTO user_division_playtime (month, player_name, discord_id)
                    VALUES (@month, @playerName, @discordId)
                    ON DUPLICATE KEY UPDATE player_name = @playerName;
                ";

                        using (var insertCmd = new MySqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@month", month);
                            insertCmd.Parameters.AddWithValue("@playerName", playerName);
                            insertCmd.Parameters.AddWithValue("@discordId", discordId);
                            await insertCmd.ExecuteNonQueryAsync();
                        }

                        // Update division playtime
                        string updateQuery = $@"
                    UPDATE user_division_playtime
                    SET {columnName} = DATE_FORMAT(SEC_TO_TIME(
                        IFNULL(TIME_TO_SEC({columnName}), 0) + @playtimeSeconds
                    ), '%H:%i:%s')
                    WHERE month = @month AND discord_id = @discordId;
                ";

                        using (var updateCmd = new MySqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@playtimeSeconds", playtimeSeconds);
                            updateCmd.Parameters.AddWithValue("@month", month);
                            updateCmd.Parameters.AddWithValue("@discordId", discordId);

                            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                            if (rowsAffected == 0)
                            {
                                Debug.WriteLine($"[UserDivision] Update failed, no row found for {discordId} {month}");
                            }
                            else
                            {
                                // After saving → show saved info
                                Debug.WriteLine(
                                    $"[UserDivision] Data saved for {playerName} ");
                            }
                        }

                        await connection.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[UserDivision] SQL Save error: {ex.Message}");
                }
            });
        }

        private readonly List<string> _afkQueue = new();

        private async Task CheckAfkStatus()
        {
            foreach (var discordId in _activeSessions.Keys)
            {
                if (!_afkQueue.Contains(discordId))
                    _afkQueue.Add(discordId);
            }

            int batchSize = 5;
            for (int i = 0; i < Math.Min(batchSize, _afkQueue.Count); i++)
            {
                string discordId = _afkQueue[0];
                _afkQueue.RemoveAt(0);

                if (!_activeSessions.TryGetValue(discordId, out var session)) continue;
                var player = session.Player;

                if (session.Player == null || !Players.Contains(session.Player)) continue;

                if (session.Branch.Equals("civ", StringComparison.OrdinalIgnoreCase) ||
                    session.Division.Equals("civ", StringComparison.OrdinalIgnoreCase) ||
                    session.Division.Equals("civilian", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var ped = API.GetPlayerPed(player.Handle);
                    var coords = API.GetEntityCoords(ped);
                    var pos = new Vector3(coords.X, coords.Y, coords.Z);

                    float distance = session.LastPosition.DistanceToSquared(pos);
                    bool moved = distance > 2.0f;
                    bool idleTooLong = (DateTime.UtcNow - session.LastMoveTime).TotalMinutes >= 15;

                    if (moved)
                    {
                        if (session.IsAfk)
                        {
                            session.IsAfk = false;
                            session.AfkDuration += DateTime.UtcNow - session.LastMoveTime;
                            player.TriggerEvent("Playtime:AfkClearedNotification");
                            Debug.WriteLine($"[AFK] {player.Name} resumed by moving.");
                        }

                        session.LastMoveTime = DateTime.UtcNow;
                        session.LastPosition = pos;
                    }
                    else if (session.IsAfk && session.IsTyping)
                    {
                        session.IsAfk = false;
                        session.AfkDuration += DateTime.UtcNow - session.LastMoveTime;
                        session.LastMoveTime = DateTime.UtcNow;
                        player.TriggerEvent("Playtime:AfkClearedNotification");
                        Debug.WriteLine($"[AFK] {player.Name} resumed by typing.");
                    }
                    else if (!session.IsAfk && !session.IsTyping && idleTooLong)
                    {
                        session.IsAfk = true;
                        Debug.WriteLine($"[AFK] {player.Name} is now marked AFK.");
                        player.TriggerEvent("Playtime:ShowAfkNotification");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[AFK] Error processing {discordId}: {ex.Message}");
                }
            }

            await Delay(0); // yield to avoid blocking next tick
        }


        private string GetDivisionColumn(string branch, string division)
        {
            var target = string.IsNullOrWhiteSpace(division) || division.ToLowerInvariant() == "none"
                ? branch
                : division;

            return target.ToLowerInvariant() switch
            {
                "afo" => "afo",
                "cid" => "cid",
                "dsu" => "dsu",
                "rpu" => "rpu",
                "npas" => "npas",
                "ert" => "ert",
                "tsg" => "tsg",
                "clinical" => "clinical",
                "clinicalstudent" => "clinical",
                "clinicaladv" => "clinicaladv",
                "hems" => "hems",
                "hart" => "hart",
                "hemsdoctor" => "hemsdoctor",
                "beepdoctor" => "hemsdoctor",
                "lfb" => "lfb",
                "fru" => "lfb",
                "highways" => "highways",
                "civ" => "civilian",
                "control" => "control",
                _ => null
            };
        }

        private void OnTypingStatusChanged([FromSource] Player player, bool isTyping)
        {
            var discordId = GetDiscordId(player);
            if (discordId == "unknown" || !_activeSessions.TryGetValue(discordId, out var session)) return;

            session.IsTyping = isTyping;
        }

        private class UserDivisionSession
        {
            public Player Player { get; set; }
            public string PlayerName { get; set; }
            public string DiscordId { get; set; }
            public string Branch { get; set; }
            public string Division { get; set; }
            public DateTime StartTime { get; set; }
            public Vector3 LastPosition { get; set; }
            public DateTime LastMoveTime { get; set; }
            public bool IsAfk { get; set; }
            public TimeSpan AfkDuration { get; set; }
            public bool IsTyping { get; set; } = false;
        }

        private class LogSessionData
        {
            public string Month { get; set; }
            public string PlayerName { get; set; }
            public string DiscordId { get; set; }
            public string Branch { get; set; }
            public string Division { get; set; }
            public int PlaytimeSeconds { get; set; }
        }
    }
}