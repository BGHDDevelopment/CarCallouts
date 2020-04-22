using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;


namespace CarCallout
{
    
    [CalloutProperties("Slow Driver Callout", "BGHDDevelopment", "0.0.4", Probability.High)]
    public class SlowDriver : Callout
    {
        private Vehicle car;
        Ped driver;

        public SlowDriver()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Slow Driver";
            CalloutDescription = "A car is driving at very slow speeds causing traffic jams.";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            API.TaskVehicleDriveWander(driver.GetHashCode(), car.GetHashCode(), 2f, 387);
            car.AttachBlip();
        }

        private string[] carList = { "speedo", "speedo2", "squalo", "stanier", "stinger", "stingergt", "stratum", "stretch", "stunt", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "predator", "oracle2", "oracle", "ninef2", "ninef", "nemesis", "minivan", "gburrito", "emperor2", "emperor"};
        public async override Task Init()
        {
            
            OnAccept();
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            Notify("~r~[CarCallouts] ~y~Suspect is driving a " + cartype);
        }

        public override void OnCancelBefore()
        { 
            foreach (Blip blip in car.AttachedBlips)
                if (blip.Exists())
                    blip.Delete();
        }
        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }
    }
}