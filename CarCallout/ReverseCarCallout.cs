using System;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;


namespace CarCallout
{
    
    [CalloutProperties("Reverse Car Callout", "BGHDDevelopment", "0.0.9", Probability.Medium)]
    public class ReverseCarCallout : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] carList = { "speedo", "speedo2", "squalo", "stanier", "stinger", "stingergt", "stratum", "stretch", "stunt", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef", "nemesis", "minivan", "gburrito", "emperor2", "emperor"};
        public ReverseCarCallout()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Car Driving in Reverse";
            CalloutDescription = "A car is driving in reverse.";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            API.TaskVehicleDriveWander(driver.GetHashCode(), car.GetHashCode(), 12f, 1923);
            car.AttachBlip();
            dynamic data1 = await GetPedData(driver.NetworkId);
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Why is everyone driving backwards?", 5000);
        }
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is driving a " + cartype + "!");
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.10;
            data.drugsUsed = new bool[] {false,false,true};
            SetPedData(driver.NetworkId,data);
            
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
        }
        public override void OnCancelBefore()
        {
        }
        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }
        private void DrawSubtitle(string message, int duration)
        {
            API.BeginTextCommandPrint("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandPrint(duration, false);
        }
    }
}