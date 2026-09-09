using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;

namespace PoliceMP.Server.Services
{
    public class QuestionService : IQuestionService
    {
        
        private readonly QuestionOptions _options;

        private static List<Question> RobberyQuestions = new()
        {
            new Question()
            {
                Text = "Where were you at the time of the robbery?",
                Attribute = "Investigate",
                PositiveAnswers = new List<string>()
                {
                    "I was at home with my family.",
                    "I can provide evidence of my whereabouts. I was attending a family gathering.",
                    "I was volunteering at a local charity event.",
                    "I was at work, and I have colleagues who can confirm my presence."
                },
                NegativeAnswers = new List<string>()
                {
                    "I prefer to remain silent until I speak with a solicitor.",
                    "I don't recall my exact whereabouts.",
                    "I was out, but not near the location of the burglary.",
                    "If I'm under arrest, I'm not answering anymore questions.",
                    "What's it to you? Have you got any evidence?"
                }
            },
            new Question()
            {
                Text = "Were you involved in the burgulary?",
                Attribute = "Investigate",
                PositiveAnswers = new List<string>()
                {
                    "No, I have no involvement in any burglary.",
                    "I'm innocent, I have no idea what you're talking about.",
                    "I was just passing by, I didn't do anything."
                },
                NegativeAnswers = new List<string>()
                {
                    "Mind your own business, copper.",
                    "You got nothing on me, pig.",
                    "You're wasting your time, I ain't saying a word."
                }
            },
            new Question()
            {
                Text = "What's your reason for being nearby?",
                Attribute = "Investigate",
                PositiveAnswers = new List<string>()
                {
                    "I live nearby and was just going about my business.",
                    "I was meeting someone in the area.",
                    "I was lost and trying to find my way.",
                    "I was walking my dog in the area.",
                    "I was out for my evening jog, helps me clear my mind after work."
                },
                NegativeAnswers = new List<string>()
                {
                    "I don't have to explain my whereabouts to you.",
                    "I plead my right to not say anything",
                    "I don't recall being in that area.",
                    "This is a free country, I can do what I want without being interrogated."
                }
            },
            new Question()
            {
                Text = "Have you been convicted of a robbery before?",
                Attribute = "Investigate",
                PositiveAnswers = new List<string>()
                {
                    "Yes, but that's got nothing to do with this.",
                    "I've paid my dues, it's in the past.",
                    "No I haven't, my records are as clean as a whistle.",
                    "I have and I paid the price, I have changed.",
                    "Whats in the past doesn't matter, I've not done anything wrong Officer."
                },
                NegativeAnswers = new List<string>()
                {
                    "None of your damn business.",
                    "Why don't you check your records and find out?",
                    "I ain't answering that without a lawyer present.",
                    "You've got no reason to ask such a question.",
                    "Screw you pig, I'm keeping quiet until I speak to a lawyer"
                }
            },
            new Question()
            {
                Text = "Have you had prior conflicts at this property?",
                Attribute = "Investigate",
                PositiveAnswers = new List<string>()
                {
                    "No, I have no issues with them.",
                    "We may have had disagreements in the past, but nothing serious.",
                    "I don't even know who lives there."
                },
                NegativeAnswers = new List<string>()
                {
                    "That's irrelevant to the case.",
                    "I'm not discussing my personal matters with you.",
                    "Ask them, not me."
                }
            }
        };
        private static List<Question> DrugQuestions = new()
        {
            new Question()
            {
                Text = "Any explanation for the drug report here?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "I'm sorry officer, but I have no involvement in any illegal activities.",
                    "I'm just a bystander here, I have no idea why you received such a call.",
                    "I'm as surprised as you are, officer. I've been minding my own business."
                },   
                NegativeAnswers = new List<string>()
                {
                    "I ain't talking to no cops. Get lost.",
                    "Maybe I was involoved, maybe not but I know dangerous people so walk on officers.",
                    "I'm just minding my own business like you should officer.",
                    "I don't have to talk to you copper, I've got more important things."
                }
            },
            new Question()
            {
                Text = "Any suspicious activities around here lately?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "Nope, everything's been quiet around here lately.",
                    "I haven't noticed anything out of the ordinary, officer.",
                    "I keep to myself, officer. Haven't seen anything suspicious.",
                    "Sorry officer, I can't help you with that."
                },   
                NegativeAnswers = new List<string>()
                {
                    "I ain't snitching on nobody, officer.",
                    "I don't know nothing, officer. Leave me alone.",
                    "I've seen a few things, but it's none of your business.",
                    "Yeah, there have been some shady characters hanging around. But I'm not snitching."
                }
            },
            new Question()
            {
                Text = "Do you consent to a search under Section 32?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "Sure officer, go ahead and search. You won't find anything.",
                    "I have nothing to hide. Go ahead and search if you want.",
                    "I don't mind a search, officer. I'm clean.",
                    "Go ahead and search, officer. You won't find any drugs on me."
                },   
                NegativeAnswers = new List<string>()
                {
                    "I ain't letting you search me, officer. Get lost.",
                    "No way, officer. You ain't searching me.",
                    "I don't care about sections. Get lost and let me get on with my day.",
                    "You have no reason to, you can't search me for standing here."
                }
            },
            new Question()
            {
                Text = "Do you have information regarding drug dealing?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "Nope, I don't know anything about that, officer.",
                    "I keep my distance from all that stuff, officer.",
                    "Sorry officer, I can't help you with that.",
                    "I don't associate with people involved in illegal activities."
                },   
                NegativeAnswers = new List<string>()
                {
                    "I ain't no snitch, officer. Figure it out yourself.",
                    "I might know a thing or two, but I ain't telling you.",
                    "Yeah, there's plenty of drug dealing going on around here.",
                    "I ain't got nothing to say to you, officer."
                }
            },
            new Question()
            {
                Text = "Do you have any drugs your person or property?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "No officer, I don't have any drugs on me.",
                    "I'm clean, officer. You won't find anything illegal on me.",
                    "I don't mess with that stuff, officer. You're barking up the wrong tree.",
                    "I have nothing illegal in my possession, officer."
                },   
                NegativeAnswers = new List<string>()
                {
                    "I ain't answering that question, officer.",
                    "I might have a little something, but you'll have to find it yourself.",
                    "You won't find anything unless you search, officer.",
                    "I might do although I know powerful people that won't be happy this won't be worth your time officer."
                }
            },
        };

        private static List<Question> BrokenDownVehicleQuestions = new()
        {
            new Question()
            {
                Text = "What seems to be the issue with your vehicle?",
                Attribute = "Investigate",
                PositiveAnswers = new List<string>()
                {
                    "I think it's a flat tire. I heard a loud hissing noise before it stopped.",
                    "The engine suddenly died and won't start again.",
                    "I noticed smoke coming from under the hood just before it broke down."
                },
                NegativeAnswers = new List<string>()
                {
                    "I have no idea. It just stopped working.",
                    "I wasn't paying attention to the vehicle before it broke down.",
                    "Why do you need to know?"
                }
            },
            new Question()
            {
                Text = "Do you require any medical assistance?",
                Attribute = "Medical Assistance",
                PositiveAnswers = new List<string>()
                {
                    "No, I'm fine. Just need help with the vehicle.",
                    "I'm not injured, just a bit shaken up.",
                    "I'll be okay once the vehicle is sorted out."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not sure, I feel a bit dizzy.",
                    "I might have hurt my back trying to push the vehicle.",
                    "I think I need to sit down for a moment."
                }
            },
            new Question()
            {
                Text = "Do you have breakdown cover?",
                Attribute = "Assistance",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I have breakdown cover. I'll call them for assistance.",
                    "I have roadside assistance through my insurance.",
                    "I'll call a friend who can help me out."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, I don't have any breakdown cover.",
                    "I didn't think I would need assistance for something like this.",
                    "I'm not sure who to call for help."
                }
            },
            new Question()
            {
                Text = "Has your vehicle recently been for a service?",
                Attribute = "Assistance",
                PositiveAnswers = new List<string>()
                {
                    "Yes, it had a service just last month. Everything seemed fine then.",
                    "I regularly maintain my vehicle, and it was serviced recently.",
                    "I had it serviced a few weeks ago, but I haven't noticed any issues until now."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, it's been a while since its last service.",
                    "I haven't had it serviced in a long time.",
                    "I've been meaning to get it serviced, but I haven't had the chance yet."
                }
            },
            new Question()
            {
                Text = "Anything out of the ordinary with your vehicle?",
                Attribute = "Pre-Breakdown Signs",
                PositiveAnswers = new List<string>()
                {
                    "Yes, there was a strange rattling noise coming from the engine recently.",
                    "I've been getting a warning light intermittently, but it usually goes away.",
                    "I noticed a burning smell a few days ago, but it went away."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, everything seemed normal before it broke down.",
                    "I haven't noticed anything out of the ordinary.",
                    "I don't pay much attention to sounds or warning lights."
                }
            }
        };

        private static List<Question> AntiSocialQuestions = new()
        {
            new Question()
            {
                Text = "Were you aware that you were disturbing others?",
                Attribute = "Awareness",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I understand that my actions were disturbing others.",
                    "I realized that my behavior might have been inappropriate.",
                    "I'm aware that I was causing a disturbance, and I apologize for it."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, I didn't think I was causing any trouble.",
                    "I didn't realize my behavior was bothering anyone.",
                    "I don't see what the problem is."
                }
            },
            new Question()
            {
                Text = "Have you taken any alcohol or drugs tonight?",
                Attribute = "Substance Use",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I've had a few drinks, but I'm not drunk.",
                    "I smoked some marijuana earlier, but I'm not under the influence.",
                    "I admit I've had a bit to drink, but it hasn't affected my behavior."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, I haven't consumed any alcohol or drugs tonight.",
                    "I'm completely sober, officer.",
                    "I don't use substances that impair my judgment."
                }
            },
            new Question()
            {
                Text = "Any personal issues impacting your behavior?",
                Attribute = "Personal Issues",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I've been under a lot of stress lately, and I may have acted out because of it.",
                    "I've been going through a rough time, but it's not an excuse for my behavior.",
                    "I acknowledge that I have personal issues, but I shouldn't have let them affect others."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, my behavior has nothing to do with personal issues.",
                    "I'm fine, I just got carried away.",
                    "I don't want to discuss my personal matters with you."
                }
            },
            new Question()
            {
                Text = "Did anyone try to stop your behavior?",
                Attribute = "Intervention",
                PositiveAnswers = new List<string>()
                {
                    "Yes, someone approached me and asked me to calm down, but I didn't listen.",
                    "I ignored warnings from bystanders to stop my behavior.",
                    "I was told by others that my actions were unacceptable, but I didn't heed their advice."
                },
                NegativeAnswers = new List<string>()
                {
                    "No one said anything to me about my behavior.",
                    "I didn't notice anyone trying to intervene.",
                    "I didn't think anyone had a problem with what I was doing."
                }
            },
            new Question()
            {
                Text = "Do you understand the impact of your actions?",
                Attribute = "Understanding",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I realize that antisocial behavior can disrupt communities and cause distress.",
                    "I'm aware of the consequences of my actions and the negative impact they can have.",
                    "I understand that my behavior affects others and I will try to be more considerate."
                },
                NegativeAnswers = new List<string>()
                {
                    "I don't see why my behavior is such a big deal.",
                    "It's not like I hurt anyone, so what's the problem?",
                    "I'm just having some fun, I didn't mean any harm."
                }
            }
        };
private static List<Question> BankArgumentQuestions = new()
        {
            new Question()
            {
                Text = "What seems to be the issue here, sir?",
                Attribute = "Cooperation",
                PositiveAnswers = new List<string>()
                {
                    "Apologies officer, I lost my temper for a moment, but I'm ready to talk calmly now.",
                    "I'm really sorry, officer. I've had a rough day, but that's no excuse for my behavior.",
                    "I understand, officer. There was a misunderstanding, but I'm more than willing to cooperate.",
                    "Sorry about that officer, I got a bit worked up. Let's resolve this peacefully.",
                    "I'm willing to work with you, officer. Can we discuss this calmly?"
                },   
                NegativeAnswers = new List<string>()
                {
                    "I ain't explaining nothing to you, copper! Get lost!",
                    "You've got some nerve questioning me, officer! Mind your own business!",
                    "Back off, pig! This ain't your concern!",
                    "I'll handle this my way, officer. You just stay out of it!",
                    "You can't tell me what to do! I'll do as I please!"
                }
            },
            new Question()
            {
                Text = "Can you tell me what led to this situation?",
                Attribute = "Cooperation",
                PositiveAnswers = new List<string>()
                {
                    "Sure officer, I just needed to withdraw some money, but there seems to be an issue with the teller.",
                    "I was trying to make a deposit, but the teller won't accept my cash. It's frustrating.",
                    "I'm here to cash a cheque, officer, but the teller is saying I can't.",
                    "I've been waiting in line for ages, officer, and now the teller is refusing to serve me.",
                    "I'm trying to transfer funds between my accounts, officer, but the teller is giving me a hard time."
                },   
                NegativeAnswers = new List<string>()
                {
                    "Why do I have to tell you anything, officer? Get lost!",
                    "I don't owe you any explanations, pig! Go find someone else to bother!",
                    "This is none of your business, officer. Back off!",
                    "I ain't saying nothing to you, copper! Go away!",
                    "You'll regret bothering me, officer. Leave me alone!"
                }
            },
            new Question()
            {
                Text = "Would you mind stepping outside with me so we can talk?",
                Attribute = "Cooperation",
                PositiveAnswers = new List<string>()
                {
                    "Sure, officer. Let's go outside and sort this out calmly.",
                    "I'm more than willing to talk outside, officer. Lead the way.",
                    "Absolutely, officer. Let's step outside and clear this up.",
                    "No problem, officer. I'll come outside with you.",
                    "I'll comply, officer. Let's step outside and discuss this peacefully."
                },   
                NegativeAnswers = new List<string>()
                {
                    "I'm not going anywhere with you, officer. Back off!",
                    "You want to take this outside? Fine by me, let's go!",
                    "Why should I go anywhere with you? I'm not moving!",
                    "I'm not stepping outside with you, pig! Get out of my face!",
                    "I don't have to listen to you, officer. Go away!"
                }
            },
            new Question()
            {
                Text = "Can you please lower your voice and remain calm?",
                Attribute = "Cooperation",
                PositiveAnswers = new List<string>()
                {
                    "Yes, officer. I'll keep calm and lower my voice.",
                    "I understand, officer. I'll speak calmly and respectfully.",
                    "Of course, officer. I apologize for raising my voice.",
                    "Absolutely, officer. I'll remain calm and cooperative.",
                    "I'll comply, officer. Let's keep this civil and calm."
                },   
                NegativeAnswers = new List<string>()
                {
                    "Why should I calm down? You're the one causing trouble!",
                    "You can't tell me what to do, officer! Mind your own business!",
                    "I'll talk however I want, officer! You can't control me!",
                    "I'm not calming down for you, pig! Get out of here!",
                    "I'll do what I want, officer! You're not the boss of me!"
                }
            },
            new Question()
            {
                Text = "Can you please leave the premises, sir?",
                Attribute = "Cooperation",
                PositiveAnswers = new List<string>()
                {
                    "Sure, officer. I'll leave peacefully. Sorry for the inconvenience.",
                    "No problem, officer. I'll step out now.",
                    "Absolutely, officer. I understand. I'll leave right away.",
                    "Of course, officer. I'll comply and leave.",
                    "I'll leave without any trouble, officer. Let me just gather my things."
                },   
                NegativeAnswers = new List<string>()
                {
                    "Why should I leave? I haven't done anything wrong!",
                    "I'm not going anywhere, officer! This is ridiculous!",
                    "You can't make me leave, pig! I have rights!",
                    "I'm staying put, officer. Deal with it!",
                    "Make me leave, if you can, officer! I'm not budging!"
                }
            },
        };

        private static List<Question> DomesticDisputeQuestions = new()
        {
            new Question()
            {
                Text = "What seems to be the cause of the dispute?",
                Attribute = "Dispute Cause",
                PositiveAnswers = new List<string>()
                {
                    "We had an argument about finances.",
                    "It started over a disagreement about parenting.",
                    "The dispute began because of unresolved issues from the past."
                },
                NegativeAnswers = new List<string>()
                {
                    "It's just a misunderstanding, nothing serious.",
                    "We were just having a heated discussion, it's under control now.",
                    "There's no real cause, things just escalated."
                }
            },
            new Question()
            {
                Text = "Was anyone physical during the dispute?",
                Attribute = "Physical Violence",
                PositiveAnswers = new List<string>()
                {
                    "Yes, things got physical, but it's not a regular occurrence.",
                    "I admit, I lost my temper and pushed my partner.",
                    "There was some pushing and shoving, but no one got seriously hurt."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, it didn't get physical. We were just arguing loudly.",
                    "We were upset, but we didn't lay hands on each other.",
                    "We know where to draw the line, it didn't escalate to violence."
                }
            },
            new Question()
            {
                Text = "Do you or your partner feel threatened or unsafe?",
                Attribute = "Safety",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I feel threatened by my partner's behavior.",
                    "I'm afraid things might escalate if we don't get help.",
                    "I don't feel safe in the current situation."
                },
                NegativeAnswers = new List<string>()
                {
                    "We're upset, but I don't feel physically threatened.",
                    "We're just arguing, we don't mean each other harm.",
                    "It's tense, but I don't fear for my safety."
                }
            },
            new Question()
            {
                Text = "Have you tried seeking support from loved ones?",
                Attribute = "Support Contact",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I called a friend to come over and help calm things down.",
                    "My partner reached out to their family for advice.",
                    "We're trying to handle it ourselves, but we might call for help if needed."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, we haven't involved anyone else.",
                    "We prefer to keep our personal matters private.",
                    "We're handling it internally, we don't need outside interference."
                }
            },
            new Question()
            {
                Text = "Are there any issues that led to this moment?",
                Attribute = "Underlying Issues",
                PositiveAnswers = new List<string>()
                {
                    "Yes, there are unresolved issues that keep resurfacing.",
                    "We've been having problems for a while, and they're coming to a head.",
                    "We need to work on communication and trust issues."
                },
                NegativeAnswers = new List<string>()
                {
                    "It's just a disagreement, it doesn't mean there are deeper problems.",
                    "Every couple argues sometimes, it's normal.",
                    "We don't need therapy, we can handle our issues on our own."
                }
            }
        };

        private static List<Question> FightInProgressQuestions = new()
        {
            new Question()
            {
                Text = "What is the cause of the altercation?",
                Attribute = "Altercation Cause",
                PositiveAnswers = new List<string>()
                {
                    "It started over a disagreement about a parking space.",
                    "The fight broke out after an argument over a game.",
                    "The altercation began due to a dispute about money."
                },
                NegativeAnswers = new List<string>()
                {
                    "None of your business! Mind your own!",
                    "Who cares what started it? We're finishing it!",
                    "You wouldn't understand even if we told you!"
                }
            },
            new Question()
            {
                Text = "Does anyone require medical help?",
                Attribute = "Injuries",
                PositiveAnswers = new List<string>()
                {
                    "Yes, there are injuries. One person has a bloody nose.",
                    "I've been punched, but it's nothing serious.",
                    "There are some bruises, but nothing too severe."
                },
                NegativeAnswers = new List<string>()
                {
                    "None of your concern! We're tough enough to handle it!",
                    "We don't need to report every scratch to you!",
                    "We're not here to discuss our injuries with you!"
                }
            },
            new Question()
            {
                Text = "Are there any weapons involved in the fight?",
                Attribute = "Weapons",
                PositiveAnswers = new List<string>()
                {
                    "Yes, one of them had a knife, but it hasn't been used.",
                    "I saw someone with a broken bottle.",
                    "We managed to disarm someone with a bat."
                },
                NegativeAnswers = new List<string>()
                {
                    "Weapons? We don't need weapons to deal with you!",
                    "We don't answer to you! Mind your own business!",
                    "Why don't you come and find out for yourself, officer?"
                }
            },
            new Question()
            {
                Text = "Do you feel in danger or threatened?",
                Attribute = "Safety",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I feel threatened by the other party.",
                    "We're outnumbered and feel unsafe.",
                    "The situation is escalating, and I fear for my safety."
                },
                NegativeAnswers = new List<string>()
                {
                    "We're not scared of anyone, especially not you!",
                    "Threatened? We're the ones doing the threatening!",
                    "We're not worried about our safety, we can take care of ourselves!"
                }
            },
            new Question()
            {
                Text = "Is there anything that caused the dispute?",
                Attribute = "Conflict History",
                PositiveAnswers = new List<string>()
                {
                    "Yes, there's a long-standing feud between us.",
                    "We've had run-ins with them before.",
                    "There's bad blood between our groups."
                },
                NegativeAnswers = new List<string>()
                {
                    "History? You could say that. But it's none of your business!",
                    "Why don't you go ask them about our history?",
                    "We don't need to explain ourselves to you, officer!"
                }
            }
        };

        private static List<Question> MentalHealthQuestions = new()
        {
            new Question()
            {
                Text = "What seems to be the problem?",
                Attribute = "Issue Description",
                PositiveAnswers = new List<string>()
                {
                    "I'm feeling overwhelmed and anxious.",
                    "I'm experiencing symptoms of a panic attack.",
                    "I'm having intrusive thoughts and can't calm down."
                },
                NegativeAnswers = new List<string>()
                {
                    "None of your business! Just leave me alone!",
                    "Why do you care? You're not here to help!",
                    "I don't need to explain myself to you!"
                }
            },
            new Question()
            {
                Text = "Have you been getting medical assistance?",
                Attribute = "Medication/Treatment",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I'm on medication prescribed by my doctor.",
                    "I've been attending therapy sessions regularly.",
                    "I have a treatment plan in place for my condition."
                },
                NegativeAnswers = new List<string>()
                {
                    "That's none of your business! I don't have to tell you anything!",
                    "What does it matter? Medication won't fix this!",
                    "I don't need treatment, I just need to be left alone!"
                }
            },
            new Question()
            {
                Text = "Have you been in touch with anyone for support?",
                Attribute = "Support Contact",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I've contacted a friend or family member for support.",
                    "I've reached out to a helpline for assistance.",
                    "I'm in touch with a mental health professional who's helping me."
                },
                NegativeAnswers = new List<string>()
                {
                    "That's none of your concern! I don't need your help!",
                    "Why would I contact anyone? No one can help me!",
                    "I don't need support from anyone, especially not you!"
                }
            },
            new Question()
            {
                Text = "Do you feel in control of yourself?",
                Attribute = "Control",
                PositiveAnswers = new List<string>()
                {
                    "No, I feel like I'm losing control of my thoughts and emotions.",
                    "I'm struggling to stay grounded and focused.",
                    "I don't trust myself to make rational decisions at the moment."
                },
                NegativeAnswers = new List<string>()
                {
                    "What's it to you? I don't have to answer your questions!",
                    "Control? Why don't you worry about controlling yourself!",
                    "I'm fine! I don't need your judgment or interference!"
                }
            }
        };

        private static List<Question> SingleSellingDrugQuestions = new()
        {
            new Question()
            {
                Text = "Why were you in the area where the drugs were found?",
                Attribute = "Location",
                PositiveAnswers = new List<string>()
                {
                    "I was just passing through, I don't have any connection to the drugs.",
                    "I live nearby, but I wasn't involved in anything illegal.",
                    "I was meeting a friend, had no idea there were drugs around."
                },
                NegativeAnswers = new List<string>()
                {
                    "I don't have to explain myself to you! I have rights!",
                    "Why do you care where I was? I'm not doing anything wrong!",
                    "I'm not answering your questions! You're just harassing me!"
                }
            },
            new Question()
            {
                Text = "Do you know anything about the drugs found at the location?",
                Attribute = "Knowledge",
                PositiveAnswers = new List<string>()
                {
                    "No, I have no idea where they came from.",
                    "I don't associate with that stuff, officer.",
                    "I'm not involved in drug dealing, if that's what you're implying."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not talking to you without a lawyer! You can't trick me!",
                    "I don't have to tell you anything! I know my rights!",
                    "You can't pin this on me! I have nothing to do with it!"
                }
            },
            new Question()
            {
                Text = "Have you ever been involved in any illegal activities?",
                Attribute = "Involvement",
                PositiveAnswers = new List<string>()
                {
                    "No, I've never been involved in anything illegal.",
                    "I've made mistakes in the past, but I'm clean now.",
                    "I used to hang out with the wrong crowd, but I've changed."
                },
                NegativeAnswers = new List<string>()
                {
                    "That's none of your business! I'm not answering that!",
                    "Why are you asking me this? I haven't done anything wrong!",
                    "You're just trying to frame me! I'm not falling for it!"
                }
            },
            new Question()
            {
                Text = "Do you associate with individuals involved in illegal activities?",
                Attribute = "Associations",
                PositiveAnswers = new List<string>()
                {
                    "No, I don't associate with criminals.",
                    "I keep to myself and stay away from trouble.",
                    "I have nothing to do with anyone involved in illegal activities."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not giving you any names! You can't make me talk!",
                    "I don't have to tell you who I associate with! It's none of your business!",
                    "Why would I tell you? You're just trying to intimidate me!"
                }
            },
            new Question()
            {
                Text = "Are you willing to answer further questions to help clear up the situation?",
                Attribute = "Willingness to Cooperate",
                PositiveAnswers = new List<string>()
                {
                    "Yes, I have nothing to hide. I'll answer any questions.",
                    "I want to clear my name, so I'll cooperate fully.",
                    "I'm willing to help with the investigation to prove my innocence."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not answering any more questions without a solicitor!",
                    "I'm done talking to you! I know my rights!",
                    "You won't get anything else from me! I'm not cooperating!"
                }
            }
        };

        private static List<Question> MissingPersonQuestions = new()
        {
            new Question()
            {
                Text = "Where are you headed?",
                Attribute = "Search",
                PositiveAnswers = new List<string>()
                {
                    "I'm trying to get to Paleto Police Station but I'm a bit lost!",
                    "Do you know where Paleto Police Station is?",
                    "There's a police station somewhere up north around here, can you help me find it?"
                },
                NegativeAnswers = new List<string>()
                {
                    "Go away pig",
                    "Why do you want to know?",
                    "I'd like to remain silent"
                }
            },
            new Question()
            {
                Text = "Do you need any medical assistance?",
                Attribute = "Medical Assistance",
                PositiveAnswers = new List<string>()
                {
                    "No, why do I look like I'm injured?",
                    "I feel fine physically, no need for medical help.",
                    "I don't think I need medical assistance right now."
                },
                NegativeAnswers = new List<string>()
                {
                    "Go away pig",
                    "Why do you want to know?",
                    "I'd like to remain silent"
                }
            },
            new Question()
            {
                Text = "What were you doing before you disappearance?",
                Attribute = "Activities Before Disappearance",
                PositiveAnswers = new List<string>()
                {
                    "I was wandering before I went missing.",
                    "Before I disappeared, I was [activity].",
                    "I remember falling down just before I forgot where I am."
                },
                NegativeAnswers = new List<string>()
                {
                    "Why do you need to know? I don't have to tell you!",
                    "I'm not talking to you! You're not going to trick me!",
                    "That's none of your business! Leave me alone!"
                }
            },
            new Question()
            {
                Text = "Do you remember seeing anyone or anything suspicious before you went missing?",
                Attribute = "Suspicious Activity",
                PositiveAnswers = new List<string>()
                {
                    "I didn't notice anything suspicious before I disappeared.",
                    "No, everything seemed normal before I went missing.",
                    "I can't recall seeing anything out of the ordinary."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not telling you anything! You're not going to frame me!",
                    "Why would I remember anything suspicious? I'm not a snitch!",
                    "You won't get any information from me! I'm not cooperating!"
                }
            },
            new Question()
            {
                Text = "Do you believe someone did this purposefully?",
                Attribute = "Potential Threats",
                PositiveAnswers = new List<string>()
                {
                    "No, I can't think of anyone who would want to harm me.",
                    "I don't believe anyone has a motive to harm me.",
                    "I don't have any enemies or conflicts that would lead to harm."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not answering that! You're trying to frame me!",
                    "That's none of your business! Stick to finding me!",
                    "I won't give you any names! I'm not a snitch!"
                }
            }
        };
        
        private static List<Question> DrunkQuestions = new()
        {
            new Question()
            {
                Text = "Do you have any details relating to a 999 call?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "I saw a guy getting really aggressive, probably drank too much.",
                    "Yeah, someone started swearing and getting real aggressive towards others.",
                    "Some guy just switched... started punching some other guy.",
                    "Some guy was in here all macho, then started swinging for this poor man."
                },
                NegativeAnswers = new List<string>()
                {
                    "No, I didn't see anything unusual tonight.",
                    "Sorry officer, I wasn't paying attention to what was happening around me.",
                    "I didn't witness anything out of the ordinary, officer.",
                    "I didn't notice any disturbances, officer."
                }
            },
            new Question()
            {
                Text = "Can you describe the person being aggressive?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "He was quite tall, had a muscular build, and seemed intoxicated.",
                    "I remember him being heavily intoxicated and causing trouble.",
                    "He seemed agitated and was swearing and yelling at everyone.",
                    "He was acting erratically, like he had lost control. Possibly on drugs or way too drunk."
                },
                NegativeAnswers = new List<string>()
                {
                    "I didn't get a good look at him, sorry.",
                    "I was too focused on getting out of there, didn't notice much.",
                    "I'm not sure, it was all happening too fast.",
                    "Sorry officer, I didn't see the person clearly."
                }
            },
            new Question()
            {
                Text = "Did anybody try to help or defuse the situation?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "Yeah, my mate Boyle stepped in to help out!",
                    "One of the bartenders rushed over and tried to break it up.",
                    "I saw security getting involved and escorting him out.",
                    "Some patrons tried to hold him back, but he was too wild."
                },
                NegativeAnswers = new List<string>()
                {
                    "I didn't notice anyone trying to stop it, sorry.",
                    "I was too far away to see if anyone intervened.",
                    "Sorry officer, I didn't see anyone trying to stop it.",
                    "I didn't see much after it started, I left the area."
                }
            },
            new Question()
            {
                Text = "Do you know if the aggressor was a regular here?",
                Attribute = "Investigation",
                PositiveAnswers = new List<string>()
                {
                    "Yeah, I've seen him around here a few times before.",
                    "He's a regular here, comes in almost every weekend.",
                    "I recognize him, but I don't know his name.",
                    "I've seen him hanging out here with his friends before."
                },
                NegativeAnswers = new List<string>()
                {
                    "I'm not sure, I haven't seen him here before.",
                    "I don't recognize him, he might not be a regular.",
                    "Sorry officer, I don't know if he's been here before.",
                    "I'm not familiar with him, he might be new here."
                }
            },
        };
        
        private static List<Question> FireInspectorQuestions = new()
        {
             new Question()
            {
                Text = "When was the last time you had a fire inspection?",
                Attribute = "Last Fire Inspection",
                PositiveAnswers = new List<string>()
                {
                    "We had a fire inspection last month.",
                    "Our last fire inspection was conducted recently.",
                    "The premises were inspected for fire safety last week.",
                    "We undergo fire inspections annually.",
                    "Our last fire inspection occurred within the past six months."
                }
            },
            new Question()
            {
                Text = "Are fire exits clearly marked?",
                Attribute = "Fire Exit Marking",
                PositiveAnswers = new List<string>()
                {
                    "Yes, all fire exits are clearly marked.",
                    "Fire exit signage is visible and compliant.",
                    "Fire exit routes are properly indicated.",
                    "Fire exit markings meet safety standards.",
                    "Fire exit signs are prominently displayed."
                }
            },
            new Question()
            {
                Text = "Are emergency lighting systems functional?",
                Attribute = "Emergency Lighting",
                PositiveAnswers = new List<string>()
                {
                    "Yes, emergency lighting is fully functional.",
                    "Emergency lighting systems are regularly tested.",
                    "Emergency lights provide adequate illumination.",
                    "Emergency lighting complies with regulations.",
                    "Emergency lighting is installed in key areas."
                }
            },
            new Question()
            {
                Text = "Are fire extinguishers easily accessible?",
                Attribute = "Fire Extinguisher Accessibility",
                PositiveAnswers = new List<string>()
                {
                    "Yes, fire extinguishers are easily accessible.",
                    "Fire extinguishers are strategically placed.",
                    "Fire extinguishers are visible and unobstructed.",
                    "Fire extinguishers are properly mounted and labeled.",
                    "Fire extinguishers are located at designated points."
                }
            },
            new Question()
            {
                Text = "Is the fire alarm system operational?",
                Attribute = "Fire Alarm System",
                PositiveAnswers = new List<string>()
                {
                    "Yes, the fire alarm system is operational.",
                    "Fire alarm tests are conducted regularly.",
                    "The fire alarm system meets safety standards.",
                    "Fire alarm panels are in good working condition.",
                    "Fire alarms are audible and clearly audible."
                }
            },
            new Question()
            {
                Text = "Is fire safety training provided to staff?",
                Attribute = "Staff Fire Safety Training",
                PositiveAnswers = new List<string>()
                {
                    "Yes, staff receive regular fire safety training.",
                    "Fire safety drills are conducted periodically.",
                    "Staff are trained in fire evacuation procedures.",
                    "Fire safety protocols are communicated to all staff.",
                    "Staff are aware of their roles in fire emergencies."
                }
            }
        };

        public QuestionService(IOptions<QuestionOptions> options)
        {
            _options = options.Value;
        }

        public List<Question> GetAll()
        {
            return _options.Questions;
        }

        public List<Question> Get(string list)
        {
            switch (list)
            {
                case "RobberyQuestions":
                    return QuestionService.RobberyQuestions;

                case "MissingPersonQuestions":
                    return QuestionService.MissingPersonQuestions;

                case "BrokenDownVehicleQuestions":
                    return QuestionService.BrokenDownVehicleQuestions;
                
                case "BankArgumentQuestions":
                    return QuestionService.BankArgumentQuestions;
                    
                case "AntiSocialQuestions":
                    return QuestionService.AntiSocialQuestions;

                case "DomesticDisputeQuestions":
                    return QuestionService.DomesticDisputeQuestions;

                case "FightInProgressQuestions":
                    return QuestionService.FightInProgressQuestions;

                case "MentalHealthQuestions":
                    return QuestionService.MentalHealthQuestions;
                
                case "SingleSellingDrugQuestions":
                    return QuestionService.SingleSellingDrugQuestions;

                case "DrunkQuestions":
                    return QuestionService.DrunkQuestions;

                case "DrugQuestions":
                    return QuestionService.DrugQuestions;
                
                case "FireInspectorQuestions":
                    return QuestionService.FireInspectorQuestions;
                    
                    

                default:
                    // Default back to normal list
                    return GetAll();
            }
        }
    }
}