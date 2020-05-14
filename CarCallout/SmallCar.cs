using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;


namespace CarCallout
{
    
    [CalloutProperties("Small Vehicle Callout", "BGHDDevelopment", "0.0.13", Probability.Low)]
    public class SmallCar : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};
        List<object> items = new List<object>();

        public SmallCar()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Very Small Vehicle";
            CalloutDescription = "A very small vehicle is causing traffic issues.";
            ResponseCode = 2;
            StartDistance = 250f;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            driver.Task.CruiseWithVehicle(car, 5f, 524675);
            car.AttachBlip();
            driver.AttachBlip();
        }
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.Tug, Location,12);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.07;
            data.drugsUsed = new bool[] {false,false,false};
            SetPedData(driver.NetworkId,data);
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            object goodItem = new {
                Name = name2,
                IsIllegal = false
            };
            items.Add(goodItem);
            data.items = items;
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