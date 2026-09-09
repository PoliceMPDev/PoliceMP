using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace PoliceMP.Shared.Enums
{
    public enum BackupType
    {
        [BackupItem(Text = "ERT", Description = "Emergency Response Team", Command = "backupert", Grade = 1)]
        Response,
        [BackupItem(Text = "RTPC", Description = "Road Transport Policing Command", Command = "backuprpu", Grade = 1)]
        Rpu,
        [BackupItem(Text = "AFO", Description = "Authorised Firearms Officer", Command = "backupafo", Grade = 1)]
        Afo,
        [BackupItem(Text = "CID", Description = "Criminal Investigation Department", Command = "backupcid", Grade = 1)]
        Cid,
        [BackupItem(Text = "Prisoner Transport", Description = "Transport To Custody", Command = "commandpt", Grade = 2)]
        PrisonerTransport,
        [BackupItem(Text = "DSU", Description = "Dog Support Unit", Command = "backupdsu", Grade = 1)]
        Dsu,
        [BackupItem(Text = "CIU", Description = "Collision Investigation Unit", Command = "backupciu", Grade = 1)]
        Ciu,
        [BackupItem(Text = "Custody", Description = "Custody Booking", Command = "backupcustody", Grade = 1)]
        Custody,
		[BackupItem(Text = "NPAS", Description = "NPAS", Command = "backupnpas", Grade = 1)]
		Npas,
		[BackupItem(Text = "Drone Unit", Description = "Drone Unit", Command = "backupdrone", Grade = 1)]
		Drone,
		[BackupItem(Text = "LHS", Description = "London Health Service", Command ="backuplhs", Grade = 1)]
        Nhs,
        [BackupItem(Text = "LFRS", Description = "London Fire & Rescue Service", Command = "backuplfrs", Grade = 1)]
        Lfb,
        [BackupItem(Text = "Motorways England", Description = "Motorways England", Command = "backupme", Grade = 3)]
        NationalHighways,
        [BackupItem(Text = "ME Recovery", Description = "ME Recovery", Command = "backupmerec", Grade = 3)]
        AARecovery,
        [BackupItem(Text = "Moderator", Description = "In Game Support", Command = "backupmod", Grade = 1)]
        Moderator,
        [BackupItem(Text = "Police Supervisor", Description = "Police Supervisor", Command = "backuppolsup", Grade = 1)]
        MetSupervisor,
        [BackupItem(Text = "LHS Team Leader", Description = "London Health Service (Team Leader)", Command = "backuplhstl", Grade = 1)]
        NhsTeamLeader,
        [BackupItem(Text = "LHS Student Paramedic", Description = "London Health Service (Student Paramedic)", Command = "backuplhssp", Grade = 2)]
        NhsClinicalStudent,
        [BackupItem(Text = "LFRS Station Officer", Description = "London Fire & Rescue Service (Station Officer)", Command = "backuplfrsso", Grade = 1)]
        LfbStationOfficer,
        [BackupItem(Text = "HART", Description = "HART", Command = "backuphart", Grade = 1)]
        HART,
        [BackupItem(Text = "HEMS", Description = "HEMS", Command = "backuphems", Grade = 1)]
        HEMS,
        [BackupItem(Text = "HEMS Doctor", Description = "HEMS", Command = "backuphemsdoctor", Grade = 1)]
        HEMSDOCTOR,
        [BackupItem(Text = "FRU", Description = "FRU", Command = "backupfru", Grade = 1)]
        FRU,
        Panic,
        CordonArea100,
        CordonArea150,
        CordonArea50,
        CordonArea100Hart,
        CordonArea150Hart,
        CordonArea50Hart
    }
}