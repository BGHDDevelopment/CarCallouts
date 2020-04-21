using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;


namespace CarCallout
{
    
    [CalloutProperties("Oversized Vehicle Callout", "BGHDDevelopment", "0.0.3", Probability.Low)]
    public class OversizedCar : Callout
    {
        private Vehicle car;
        Ped driver;

        public OversizedCar()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Oversized Vehicle";
            CalloutDescription = "A oversized car is causing issues.";
            ResponseCode = 2;
            StartDistance = 250f;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            API.TaskVehicleDriveWander(driver.GetHashCode(), car.GetHashCode(), 15f, 525116);
            car.AttachBlip();
        }
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.Dump, Location,12);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
        }
        public override void OnCancelBefore()
        {
            car.AttachedBlip.Delete();
        }
        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }
    }
}