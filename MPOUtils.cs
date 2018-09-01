using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using System.IO;

namespace Anaglyph3DS
{
    class MPOUtils
    {
        public static List<Image> GetJPEGImages(string path)
        {
            List<Image> srcList = new List<Image>();

            foreach (byte[] img in ExtractJPEGFromMPO(path))
            {
                var ms = new MemoryStream(img);
                var src = new Bitmap(ms);

                Console.WriteLine("Got an image whose dimensions are " + src.Width + "x" + src.Height + " pixels!");



                srcList.Add(new Bitmap
                    (src));
            }

            return srcList;

        }


        public static List<byte[]> ExtractJPEGFromMPO(String fileName)
        {
            FileStream file = File.OpenRead(fileName);
            BinaryReader br = new BinaryReader(file);

            List<byte[]> images = new List<byte[]>();
            List<MPEntry> mpentries = new List<MPEntry>();

            UInt16 SOI_MAGIC = IO.readUSHORT(br);

            if (SOI_MAGIC == MagicMarker.START_OF_IMAGE)
            {
                Console.WriteLine("Match found.");//+ IO.hexy(SOI_MAGIC) + " " + IO.hexy(MagicMarker.START_OF_IMAGE));

                while (file.Position < file.Length)
                {
                    UInt16 appSegMarker, segLen;

                    appSegMarker = IO.readUSHORT(br);
                    segLen = IO.readUSHORT(br);

                    //  Console.WriteLine(IO.hexy(segLen) + "!"+segLen);

                    // Console.WriteLine(IO.hexy(appSegMarker) + " " + IO.hexy(MagicMarker.APP2) + file.Position);

                    if (appSegMarker == MagicMarker.APP2)
                    {
                        Console.WriteLine("APP2 Block Found.");

                        segLen = (ushort)(segLen - 4);

                        if (Encoding.ASCII.GetString(br.ReadBytes(4)) == MagicMarker.MP_EXTENSIONS_ID_STRING)
                        {
                            Console.WriteLine("This APP2 Block is an Multiple Picture Object Extensions Section!");

                            Int64 extens_start = file.Position;
                            bool is_section_LE = ((IO.readUINT(br) == MagicMarker.LEMP) ? true : false);

                            if (is_section_LE)
                                Console.WriteLine("Little endian is specified.");

                            Int64 primary_IFD_offset = IO.readUINT(br, is_section_LE);

                            file.Position = extens_start + primary_IFD_offset;

                            UInt16 tagcunt = IO.readUSHORT(br, is_section_LE);

                            String MPO_VersID;
                            UInt32 nImgs, mpEntry, imgList, nFrames;

                            nImgs = mpEntry = imgList = nFrames = 0;

                            Console.WriteLine("There are " + tagcunt + "tags. ");

                            for (int i = 0; i < tagcunt; i++)
                            {
                                UInt16 tagID = IO.readUSHORT(br, is_section_LE);

                                br.ReadBytes(2 + 4);

                                switch (tagID)
                                {
                                    case 45056:
                                        MPO_VersID = Encoding.ASCII.GetString(br.ReadBytes(4));
                                        Console.WriteLine("MPO Version String is " + MPO_VersID + ".");
                                        Form1.meta.Add("MPO Version", MPO_VersID);
                                        break;
                                    case 45057:
                                        nImgs = IO.readUINT(br, is_section_LE);
                                        Console.WriteLine("There are " + nImgs + " images.");
                                   //     Form1.meta.Add("ImgCount", nImgs.ToString());
                                        break;
                                    case 45058:
                                        mpEntry = IO.readUINT(br, is_section_LE);
                                        Console.WriteLine("MP ENTRY");
                                        for (int j = 0; j < nImgs; j++)
                                        {
                                            //mpentries.Add(new MPEntry(br.ReadBytes(16)));
                                        }
                                        break;
                                    case 45059:
                                        imgList = IO.readUINT(br, is_section_LE);
                                        break;
                                    case 45060:
                                        nFrames = IO.readUINT(br, is_section_LE);
                                        Console.WriteLine("There are " + nFrames + " frames.");
                                        Form1.meta.Add("NFrames", nFrames.ToString());
                                        break;
                                    case 45576:
                                        Console.WriteLine("HAD:" + BitConverter.ToSingle(br.ReadBytes(4), 0));
                                        break;
                                    case 45575:
                                        float divang = BitConverter.ToSingle(br.ReadBytes(4), 0);
                                        Console.WriteLine("Divergence Angle:" + divang );
                                        Form1.meta.Add("DivAngle", divang.ToString());
                                        break;

                                    default:
                                        float sin = BitConverter.ToSingle(br.ReadBytes(4), 0);
                                        Console.WriteLine("Metadata Tag No. " + tagID + ": " + BitConverter.ToSingle(br.ReadBytes(4), 0));
                                        Form1.meta.Add(tagID.ToString(), sin.ToString());
                                        break;
                                }

                            }

                            var offsetNext = IO.readUINT(br, is_section_LE);

                            // Read the values of the MP Index IFD
                            for (uint i = 0; i < nImgs; i++)
                            {
                                // var bytes = r.ReadBytes(16);
                                // var iattr = IO.readUINT(br, is_section_LE);
                                br.ReadBytes(4);

                                var imageSize = IO.readUINT(br, is_section_LE);
                                var dataOffset = IO.readUINT(br, is_section_LE);

                                //var d1EntryNo = IO.readUSHORT(br, is_section_LE);
                                //var d2EntryNo = IO.readUSHORT(br, is_section_LE);

                                br.ReadBytes(4);

                                // Calculate offset from beginning of file
                                long offset = i == 0 ? 0 : dataOffset + extens_start;

                                // store the current position
                                long jumpBack = file.Position;

                                // read the image
                                file.Position = offset;

                                images.Add(br.ReadBytes((int)imageSize));

                                file.Position = jumpBack;
                            }

                            int attrtagcnt = BitConverter.ToUInt16(br.ReadBytes(2), 0);

                            Console.WriteLine(attrtagcnt + " attribute tags here!");


                            //br.ReadBytes(4);

                            Console.WriteLine("Offset at " + file.Position);




                            for (int i = 0; i < tagcunt + 1; i++)
                            {
                                UInt16 tagID = IO.readUSHORT(br, is_section_LE);

                                br.ReadBytes(2 + 4);

                                Console.WriteLine("TagID is" + tagID);

                                switch (tagID)
                                {
                                    case 45572:

                                        Console.WriteLine("Base Viewpoint Number:" + br.ReadUInt32());

                                        break;

                                    case 45313:

                                        Console.WriteLine("MP Individual Image Number:" + br.ReadUInt32());
                                        break;

                                    case 45573:

                                        Console.WriteLine("Angle:" + br.ReadInt32() + "/" + br.ReadInt32());
                                        break;

                                    case 45574:

                                        Console.WriteLine("Baseline Length:" + br.ReadUInt32() / br.ReadUInt32());
                                        break;


                                }
                                Console.WriteLine("Offset at " + file.Position);
                            }




                        }




                    }

                    br.ReadBytes(segLen - 2);


                }



            }
            else
            {
                Console.WriteLine("SOI (START OF IMAGE) Marker missing. Aborting." + BitConverter.ToString(BitConverter.GetBytes(SOI_MAGIC), 0) + BitConverter.ToString(BitConverter.GetBytes(MagicMarker.START_OF_IMAGE), 0));
            }


            return images;
        }

    }
}
