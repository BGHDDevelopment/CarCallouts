using System;
using System.Data;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;


namespace CarCallout
{

    [CalloutProperties("Pursuit of Armed Suspects", "BGHDDevelopment", "0.0.5", Probability.Low)]
    public class PursuitCallout : Callout
    {
        private Vehicle car;
        Ped driver;
        Ped passenger;

        private string[] carList =
        {
            "speedo", "speedo2", "squalo", "stanier", "stinger", "stingergt", "stratum", "stretch", "stunt", "taco",
            "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi",
            "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "predator",
            "oracle2", "oracle", "ninef2", "ninef", "nemesis", "minivan", "gburrito", "emperor2", "emperor"
        };

        public PursuitCallout()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Pursuit of Armed Suspects";
            CalloutDescription = "Suspects just robbed a person with weapons. They are fleeing.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            passenger.Weapons.Give(WeaponHash.Pistol, 20, true, true);
            driver.Weapons.Give(WeaponHash.SMG, 30, true, true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("The driver and passenger are fleeing!");
            car.AttachBlip();
            API.Wait(6000);
            passenger.Task.FightAgainst(player);
        }

        public async override Task Init()
        {

            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            passenger = await SpawnPed(GetRandomPed(), Location + 1);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            passenger.SetIntoVehicle(car, VehicleSeat.Passenger);
            Notify("~r~[CarCallouts] ~y~Suspect are driving a " + cartype);

            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            passenger.AlwaysKeepTask = true;
            passenger.BlockPermanentEvents = true;
        }

        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }

        public override void OnCancelBefore()
        {
            foreach (Blip blip in car.AttachedBlips)
                if (blip.Exists())
                    blip.Delete();
        }
    }
}