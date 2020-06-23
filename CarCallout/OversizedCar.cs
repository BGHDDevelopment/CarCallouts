using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace CarCallout
{
    
    [CalloutProperties("Oversized Vehicle Callout", "BGHDDevelopment", "0.0.17")]
    public class OversizedCar : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};
        List<object> items = new List<object>();

        public OversizedCar()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Oversized Vehicle";
            CalloutDescription = "A oversized vehicle is causing issues.";
            ResponseCode = 2;
            StartDistance = 250f;
            UpdateData();
        }
        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            driver.Task.CruiseWithVehicle(car,15f, 525116);
            car.AttachBlip();
            driver.AttachBlip();
        }
        public async override Task OnAccept()
        {
            InitBlip();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.Dump, Location,12);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.13;
            data.drugsUsed = new bool[] {true,false,true};
            Utilities.SetPedData(driver.NetworkId, data);
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            object goodItem = new {
                Name = name2,
                IsIllegal = false
            };
            items.Add(goodItem);
            data.items = items;
            Utilities.SetPedData(driver.NetworkId, data);
            //Car Data
            dynamic vehicleData = new ExpandoObject();
            vehicleData.insurance = false;
            vehicleData.registration = false;
            Utilities.SetVehicleData(car.NetworkId,vehicleData);
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