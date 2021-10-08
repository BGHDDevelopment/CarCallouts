using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;


namespace CarCallout
{
    
    [CalloutProperties("Bus Pursuit", "BGHDDevelopment", "1.1")]
    public class BusPursuit : Callout
    {
        private Vehicle car;
        Ped driver;
        public BusPursuit()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Bus Pursuit";
            CalloutDescription = "A bus has been stolen!";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            Random random = new Random();
            car = await SpawnVehicle(VehicleHash.Bus, Location,12);
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            VehicleData datacar = await Utilities.GetVehicleData(car.NetworkId);
            string vehicleName = datacar.Name;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are driving a " + vehicleName + "!");
            
            driver.Task.CruiseWithVehicle(car, 2f, 387);
            car.AttachBlip();
            driver.AttachBlip();
            API.AddBlipForEntity(car.GetHashCode());
            API.AddBlipForEntity(driver.GetHashCode());
        }
        
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
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