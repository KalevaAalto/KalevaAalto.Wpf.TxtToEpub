using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using VersOne.Epub;

namespace KalevaAalto.Wpf.TxtToEpub.Services
{
    public class TestService
    {
        public static async Task Test()
        {

            XNamespace ns = @"urn:oasis:names:tc:opendocument:xmlns:container";
            XDocument xml = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement(ns + @"container",
                            new XAttribute(@"version", @"1.0"),
                            new XElement(ns + @"rootfiles",
                                new XElement(ns + @"rootfile",new XAttribute(@"full-path", @"OPS/content.opf"), new XAttribute(@"media-type", @"application/oebps-package+xml"))
                                )
                            )
                    );
            //await xml.SaveAsync(new FileStream(Path.Combine(@"")));
            Trace.WriteLine(xml.ToString(SaveOptions.OmitDuplicateNamespaces));

            //string? fileName = OpenFile(@"Epub Documents(*.epub)|*.epub");
            //if (string.IsNullOrEmpty(fileName)) { return; }

            //EpubBook epubBook = EpubReader.ReadBook(fileName);

            //Trace.WriteLine($"标题：{epubBook.Title}");



            //foreach (var chapter in epubBook.ReadingOrder.Where(it=>it.ContentFileType == EpubContentFileType.TEXT))
            //{
            //    Trace.WriteLine($"Chapter ContentFileType: {chapter.ContentFileType}");
            //    Trace.WriteLine($"Chapter ContentType: {chapter.ContentType}");
            //    Trace.WriteLine($"Content: {chapter.Content}");
            //    //break;
            //}



        }


    }
}
