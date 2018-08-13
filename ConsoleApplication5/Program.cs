using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in GetRates())
            {
                Console.WriteLine(item);
            }


            // GetExchangeRates();

            //CreateShortVersion();
        }

        static void CreateShortVersion()
        {
            XmlDocument doc = new XmlDocument();

            //корневой элемент
            XmlElement root = doc.CreateElement("orders");

            //
            XmlElement order = doc.CreateElement("order");

            XmlElement price = doc.CreateElement("price");
            //до </price>

            price.InnerText = "3562";
            //после <price>3562</price>

            XmlAttribute currency = doc.CreateAttribute("currency");
            currency.InnerText = "KZT";
            price.Attributes.Append(currency);
            //после <price currency="KZT">3562</price>


            XmlAttribute discount = doc.CreateAttribute("discount");
            discount.InnerText = "2";
            price.Attributes.Append(discount);
            //после <price currency="KZT" discount="2">3562</price>


            order.AppendChild(price);
            root.AppendChild(order);
            doc.AppendChild(root);

            doc.Save("orders.xml");
            Console.WriteLine("ok");

        }


        static XmlDocument GetExchangeRates()
        {
            XmlDocument shortRates = new XmlDocument();
            XmlElement root = shortRates.CreateElement("rates");



            XmlDocument doc = new XmlDocument();


            try
            {
                doc.Load("http://www.nationalbank.kz/rss/rates.xml");
                if (doc.HasChildNodes)
                {
                    #region xml1
                    //вариант первый поиска item
                    // foreach (XmlNode item in doc.ChildNodes[1])
                    // {
                    //     Console.WriteLine(item.Name);
                    //     if (item.HasChildNodes)
                    //     {
                    //         foreach (XmlNode channel in item.ChildNodes)
                    //         {
                    //             Console.WriteLine("\t{0}", channel.Name);
                    //             if (channel.Name.Equals("item"))
                    //             {
                    //                 foreach (XmlNode chItem in channel.ChildNodes)
                    //                 {
                    //                     Console.WriteLine("\t\t{0}", chItem.Name);
                    //                 }

                    //             }
                    //         }
                    //     }
                    //} 
                    #endregion

                    #region xml2
                    //второй способ поиска item
                    foreach (XmlNode rootItem in doc.SelectNodes("rss/channel/item"))
                    {
                        //Console.WriteLine(root.Name);
                        //Console.WriteLine(rootItem.SelectSingleNode("title").InnerText);

                        string rateName = rootItem.SelectSingleNode("title").InnerText;
                        string rateDescription = rootItem.SelectSingleNode("description").InnerText;

                        XmlElement rate = shortRates.CreateElement(rateName);
                        rate.InnerText = rateDescription;
                        //<EUR>408.89</EUR>

                        XmlAttribute pubDateAtr = shortRates.CreateAttribute("pubDate");
                        pubDateAtr.InnerText = rootItem.SelectSingleNode("pubDate").InnerText;

                        rate.Attributes.Append(pubDateAtr);
                        //<EUR pubDate = "13.08.18">408.89</EUR>

                        root.AppendChild(rate);
                    }
                    shortRates.AppendChild(root);
                    shortRates.Save("shortRates.xml");
                }
                return doc;
                #endregion


            }
            catch (Exception)
            {
                return null;
            }






        }

        static List<Rate> GetRates()
        {
            List<Rate> rates = new List<Rate>();

            //XmlDataDocument doc = GetExchangeRates();
            //foreach (XmlNode root in doc.SelectNodes("rss/channel/item"))
            //{
            //}

            foreach (XmlNode root in GetExchangeRates().SelectNodes("rss/channel/item"))
            {
                Rate rate = new Rate();
                rate.title = root.SelectSingleNode("title").InnerText;
                rate.pubDate = DateTime.Parse(root.SelectSingleNode("pubDate").InnerText);
                rate.description = double.Parse(root.SelectSingleNode("description").InnerText.Replace(".", ","));
                rates.Add(rate);
            }


            return rates;
        }

        public class Rate
        {
            public string title { get; set; }
            public DateTime pubDate { get; set; }
            public double description { get; set; }

            public override string ToString()
            {
                string str = string.Format("{0} - {1}", title, description);
                return str;
            }
        }
    }
}
