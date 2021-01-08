using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
    class MissionIdentifiers
    {
        public static Dictionary<string, string> missionScripts = new Dictionary<string, string> {
            {"prologue1", "Prologue"},
            {"armenian1", "Franklin & Lamar"},
            {"armenian2", "Repossesion"},
            {"franklin0", "Chop"},
            {"armenian3", "Complications"},
            {"family1", "Father/Son"},
            {"family3", "Marriage Counseling"},
            {"lester1", "Friend Request"},
            {"family2", "Daddy's Little Girl"},
            {"jewelry_setup1", "Casing the Jewel Store"},
            {"jewelry_prep1b", "Carbine Rifles"},
            {"jewelry_prep1a", "Bugstars Equipment"}, //Let the fun begin
            {"jewelry_prep2a", "BZ Gas Grenades"}, //...
            {"jewelry_heist", "The Jewel Store Job"},
            {"lamar1", "The Long Stretch"},
            {"trevor1", "Mr. Philips"},
            {"chinese1", "Trevor Philips Industries"},
            {"trevor2", "Nervous Ron"},
            {"chinese2", "Crystal Maze"},
            {"trevor3", "Friends Reunited"},
            {"family4", "Fame or Shame"},
            {"fbi1", "Dead Man Walking"},
            {"fbi2", "Three's Company"},
            {"franklin1", "Hood Safari"},
            {"docks_setup", "Scouting the Port"},
            {"family5", "Did Somebody Say Yoga?"},
            {"fbi3", "By The Book"},
            {"docks_prep1", "Minisub"},
            {"docks_prep2b", "Cargobob"}, //...
            {"fbi4_intro", "Blitz Play Intro"},
            {"fbi4_prep1", "Garbage Truck"},
            {"fbi4_prep2", "Tow Truck"},
            {"fbi4_prep4", "Masks"},
            {"fbi4_prep5", "Boiler Suits"},
            {"docks_heista", "Merryweather Heist (Freighter)"},
            {"docks_heistb", "Merryweather Heist (Offshore)"},
            {"fbi4", "Blitz Play"},
            {"carsteal1", "I Fought The Law"},
            {"carsteal2", "Eye in the Sky"},
            {"solomon1", "Mr. Richards"},
            {"martin1", "Caida Libre"},
            {"carsteal3", "Deep Inside"},
            {"exile1", "Minor Turbulence"},
            {"rural_bank_setup", "Paleto Score Setup"},
            {"exile2", "Predator"},
            {"rural_bank_prep1", "Military Hardware"},
            {"rural_bank_heist", "Paleto Score"},
            {"exile3", "Derailed"},
            {"fbi5a", "Monkey Business"},
            {"trevor4", "Hang Ten"},
            {"finale_heist1", "Surveying the Score"},
            {"michael1", "Bury the Hatchet"},
            {"carsteal4", "Pack Man"},
            {"michael2", "Fresh Meat"},
            {"solomon2", "Ballad of Rocco"},
            {"agency_heist1", "Cleaning out the Bureau"},
            {"family6", "Reuniting the Family"},
            {"agency_prep1", "Fire Truck" }, //...
            {"agency_heist2", "Architect's Plans"},
            {"agency_heist3a", "Bureau Raid (Covert)"},
            {"agency_heist3b", "Bureau Raid (Roof)"},
            {"solomon3", "Legal Trouble"},
            {"michael3", "The Wrap Up"},
            {"franklin2", "Lamar Down"},
            {"michael4", "Meltdown"},
            //{"finale_heist2_intro", "Big Score Intro"}, //Not the fucking mission name
            {"finale_heist2_intro", "Planning the Big Score"},
            {"finale_heist_prepc", "Gauntlet"},
            {"finale_heist_prepa", "Stingers"},
            {"finale_heist_prepb", "Driller"}, //...
            {"finale_heist_prepd", "Sidetracked"}, //...
            {"finale_heist2a", "The Big Score (Subtle)"},
            {"finale_heist2b", "The Big Score (Obvious)"},
            {"finalea", "Something Sensible"}, //Really, no finales?
            {"finaleb", "The Time's Come"},
            {"finalec1", "The Third Way [Foundry]"},
            {"finalec2", "The Third Way [PostFoundry]"}, //Shame...
            {"assassin_valet", "The Hotel Assassination"},		
            {"assassin_multi", "The Multi-Target Assassination"},		
            {"assassin_hooker", "The Vice Assassination"},
            {"assassin_bus", "The Bus Assassination"},
            {"assassin_construction", "The Construction Assassination"},
            //Technially sidemissions
            {"me_amanda1", "The Good Husband"}, //Ok, I don't blame you for these...
            {"me_jimmy1", "Parenting 101"},
            {"me_tracey1", "Doting Dad"},
            {"pilot_school", "Flight School"},
            //Taxis
            {"taxi_clowncar", "Clown Car"},
            {"taxi_cutyouin", "Cut You In"},
            {"taxi_deadline", "Deadline"},
            {"taxi_takeiteasy", "Take It Easy"},
            {"taxi_needexcitement", "Fare Needs Excitement"},
            {"taxi_followcar", "Follow Car"},
            {"taxi_gotyounow", "Got You Now" },
            {"taxi_gotyourback", "Got Your Back"},
            {"taxi_taketobest", "Take to the Best Tailor"},
            //Therapy
            {"drf1", "Chaos"},
            {"drf2", "Evil"},
            {"drf3", "Negativity"},
            {"drf4", "Fucked Up"},
            {"drf5", "Abandonment Issues"}

        };

        public static Dictionary<string, string> freaksScripts = new Dictionary<string, string> {
            { "abigail1","Death at Sea"}, //Not required for 100% so fair...
            { "abigail2", "What Lies Beneath"},
            { "barry1", "Grass Roots - Michael"}, //We need them aliens
            { "barry2", "Grass Roots - Trevor"}, //And them clowns
            { "tonya1", "Pulling Favours"},
            { "tonya2", "Pulling Another Favour"},
            { "tonya3", "Pulling Favours Again"},
            { "tonya4", "Still Pulling Favours"},
            { "tonya5", "Pulling One Last Favour"},
            { "hao1", "Shift Work"},
            { "paparazzo1", "Paparazzo"},
            { "paparazzo2", "Paparazzo - The Sex Tape"},
            { "paparazzo3", "Paparazzo - The Partnership"}, //idk why its a seperate mission
            { "paparazzo3a", "Paparazzo - The Meltdown"},
            { "paparazzo4", "Paparazzo - Reality Check"},
            { "omega1", "Far Out"},
            { "omega2", "The Final Frontier"},
            { "barry3a", "Grass Roots - The Pickup"},
            { "barry4", "Grass Roots - The Smoke-in"},
            { "extreme1", "Risk Assesstment"},
            { "extreme2", "Liqudity Risk"},
            { "extreme3", "Targeted Risk"},
            { "extreme4", "Uncalculated Risk"},
            { "fanatic3", "Exercising Demons - Franklin"},
            { "dreyfuss1", "A Starlet in Vinewood"},
            { "minute1", "The Civil Border Patrol"},//`MURICA
            { "minute2", "An American Welcome"},//[EXPLITIVE REDACTED]
            { "minute3", "Minute Man Blues"},//YEAH!
            { "hunting1", "Target Practice"},//Cletus: Hey bud
            { "hunting2", "Fair Game"}, // [TANK AQUIRED]
            { "josh1", "Extra Commission"},//...
            { "josh2", "Closing the Deal"},
            { "josh3", "Surreal Estate"},
            { "josh4", "Breach of Contract"},//Wait... there were four of these?
            { "fanatic1", "Exercising Demons - Michael"},
            { "fanatic2", "Exercising Demons - Trevor"},
            { "maude1", "Bail Bonds"},
            { "mrsphilips1", "Mrs. Philips"}, //Your mother is disapointed with you
            { "mrsphilips2", "Damaged Goods"},
            { "nigel1", "Nigel and Mrs. Thornhill"},
            { "nigel1a", "Vinewood Souvenirs - Willy"},
            { "nigel1b", "Vinewood Souvenirs - Tyler"},
            { "nigel1c", "Vinewood Souvenirs - Kerry"},
            { "nigel1d", "Vinewood Souvenirs - Mark"},
            { "nigel2", "Vinewood Souvenirs - Al Di Napoli"},
            { "nigel3", "Vinewood Souvenirs - The Last Act"},
            { "rampage1", "Rampage: Rednecks"},
            { "rampage2", "Rampage: Vagos"},
            { "rampage3", "Rampage: Ballas"},
            { "rampage4", "Rampage: Military"},
            { "rampage5", "Rampage: Hipsters"},
            { "thelastone", "The Last One" }, //The Last One
            //Epsilon%
            { "epsilon1", "Seeking the Truth"},//There is actually a need for these now
            { "epsilon2", "Accepting the Truth"},
            { "epsilon3", "Assuming the Truth"},
            { "epsilon4", "Chasing the Truth"},
            { "epsilon5", "Bearing the Truth"},
            { "epsilon6", "Delivering the Truth"},
            { "epsilon7", "Exercising the Truth"},
            { "epsilon8", "Unknowing the Truth"}
        };
        public static Tuple<string,bool> getMissionInfo(string scriptName)
        {
            if (missionScripts.ContainsKey(scriptName))
            {
                return new Tuple<string, bool>(missionScripts[scriptName], false);
            }
            else if (freaksScripts.ContainsKey(scriptName))
            {
                return new Tuple<string, bool>(freaksScripts[scriptName], true);
            }
            return new Tuple<string, bool>(scriptName, false);
        }
        public static List<string> getAllMissions()
        {
            var missions = new List<string>();
            missions.AddRange(missionScripts.Values);
            missions.AddRange(freaksScripts.Values);
            return missions;
        }
    }
}
