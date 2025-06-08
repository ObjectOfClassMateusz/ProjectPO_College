using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets;

using ProtoBuf;//serializacja


namespace Models
{
    public enum ElectronicSI
    {
        Wat='W',
        Voltage='V',
        Amper='A',
        Byte='B'
    };
    public enum Category
    {
        TTLs,
        Gaming,
        Home,
        Kichen,
        Motor
    }

    [ProtoContract]
    public class Barcode
    {
        [ProtoMember(1)]
        private Byte code          {get;set;}

        public  Barcode(Byte code) 
        {this.code = code;}
        public  Byte getCode()     
        {return code;}
    };

    internal interface IProduct
    {
        Tuple<string,string,decimal?> getData();
        Barcode getBarcode();
        void setPrice(decimal price);
    }
    internal interface IElectronics
    {
        Dictionary<string, string> getTechnicalSpecifications();
        decimal? getPower();
        decimal? getCurrent();
        decimal? getVoltage();
        decimal? getLogicThreading();
    }

    [ProtoContract]
    public class Product : IProduct
    {
        [ProtoMember(1)]
        protected Barcode id;
        [ProtoMember(2)]
        protected decimal? price {get;set;}
        [ProtoMember(3)]
        protected string name {get;set;}
        [ProtoMember(4)]
        protected string description {get;set;}
        [ProtoMember(5)]
        protected Category category { get;set;}
        [ProtoMember(6)]
        protected ASCIImage image {get;set;}

        public string Name
        {
            get { return name; }
        }
        public string Category
        {
            get { return category.ToString(); }
        }
        public string Description
        {
            get { return description; }
        }
        public decimal? Price
        {
            get { return price; }
        }
        public Tuple<string, string, decimal?> getData()
        {
            return new Tuple<string, string, decimal?>(this.name,this.description,this.price);
        }
        public Barcode getBarcode()
        {
            return id;
        }
        public void setPrice(decimal price)
        {
            this.price = price;
        }
        public Product(Barcode barcode , string name , string desc , decimal price,Category category , ASCIImage img)
        {
            this.id = barcode;
            this.name = name;
            this.description = desc;
            this.price = price;
            this.category = category;
            this.image = img;
        }



    }

    [ProtoContract]
    public class Device : Product , IElectronics
    {
        [ProtoMember(1)]
        public Dictionary<string,string> techSpecification { get; set; }
        [ProtoMember(2)]
        protected Tuple<decimal?, ElectronicSI> power { get; set; }
        [ProtoMember(3)]
        protected Tuple<decimal?, ElectronicSI> voltage { get; set; }
        [ProtoMember(4)]
        protected Tuple<decimal?, ElectronicSI> current { get; set; }
        [ProtoMember(5)]
        protected Tuple<decimal?, ElectronicSI> logicthread { get; set; }


        public decimal? getPower()
        {
            return 5.0M;
        }
        public decimal? getVoltage()
        {
            return 16M;
        }
        public decimal? getCurrent()
        {
            return 16M;
        }
        public decimal? getLogicThreading()
        {
            return 16M;
        }
        public Tuple<string, string, decimal?> getData()
        {
            return new Tuple<string, string, decimal?>("","",0M);
        }
        public Dictionary<string, string> getTechnicalSpecifications()
        {
            return techSpecification;
        }
        public Barcode getBarcode()
        {
            return id;
        }
        public Device(Barcode barcode , string name , string desc , decimal price, Category category, ASCIImage img, decimal wats, decimal volts, decimal ampesrs, decimal bytes) : base(barcode,  name,  desc,  price, category , img)
        {
            this.power = new Tuple<decimal?, ElectronicSI>(wats, ElectronicSI.Wat);
            this.voltage = new Tuple<decimal?, ElectronicSI>(volts, ElectronicSI.Voltage);
            this.current = new Tuple<decimal?, ElectronicSI>(ampesrs, ElectronicSI.Amper);
            this.logicthread = new Tuple<decimal?, ElectronicSI>(bytes, ElectronicSI.Byte);
        }
    }
    public static class ProductSearch
    {
        public static List<Product> SearchByKeyword(IEnumerable<Product> products, string keyword)
        {
            keyword = keyword.ToLower();

            return products.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(keyword)) ||
                (p.Description != null && p.Description.ToLower().Contains(keyword))
            ).ToList();
        }

        public static List<Product> SearchByCategory(IEnumerable<Product> products, Category category)
        {
            return products.Where(p => p.Category == category.ToString()).ToList();
        }
    }
}
