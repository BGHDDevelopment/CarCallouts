using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;


namespace CarCallout
{
    
    [CalloutProperties("Stolen Firetruck Callout", "BGHDDevelopment", "1.0.1")]
    public class StolenFireTruck : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};
        public StolenFireTruck()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Stolen Firetruck";
            CalloutDescription = "Someone stole a firetruck!";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.FireTruk, Location,12);
            API.SetVehicleLights(car.GetHashCode(), 2);
            API.SetVehicleLightsMode(car.GetHashCode(), 2);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            PedData data = new PedData();
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            List<Item> items = new List<Item>();
            Item goodItem = new Item {
                Name = name2,
                IsIllegal = false
            };
            items.Add(goodItem);
            data.Items = items;
            Utilities.SetPedData(driver.NetworkId,data);
            //Car Data
            VehicleData vehicleData = await Utilities.GetVehicleData(car.NetworkId);
            Utilities.SetVehicleData(car.NetworkId,vehicleData);
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId,true);
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 30f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            car.AttachBlip();
            driver.AttachBlip();
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is fleeing!");
            Pursuit.RegisterPursuit(driver);
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
    }
}