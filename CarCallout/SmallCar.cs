using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;


namespace CarCallout
{
    
    [CalloutProperties("Small Vehicle Callout", "BGHDDevelopment", "1.0.0")]
    public class SmallCar : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};

        public SmallCar()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Very Small Vehicle";
            CalloutDescription = "A very small vehicle is causing traffic issues.";
            ResponseCode = 2;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.Airtug, Location,12);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            //Driver Data
            PedData data = new PedData();
            data.BloodAlcoholLevel = 0.07;
            List<Item> items = new List<Item>();
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            Item goodItem = new Item {
                Name = name2,
                IsIllegal = false
            };
            items.Add(goodItem);
            data.Items = items;
            Utilities.SetPedData(driver.NetworkId,data);
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            
            driver.Task.CruiseWithVehicle(car, 5f, 524675);
            car.AttachBlip();
            driver.AttachBlip();
        }
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
        }
    }
}