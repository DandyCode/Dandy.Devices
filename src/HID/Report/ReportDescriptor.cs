using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dandy.Devices.HID.Report
{
    public static class ReportDescirptor
    {
        public static IEnumerable<ReportCollection> Parse(byte[] bytes)
        {
            using (var reader = new BinaryReader(new MemoryStream(bytes))) {
                var descriptor = new ReportDescriptorData();
                var descriptorStack = new Stack<ReportDescriptorData>();
                ReportCollection collection = null;

                while (reader.BaseStream.Position < bytes.Length) {
                    var header = reader.ReadByte();
                    var size = 1 << (header & 0b11) >> 1;
                    var data = 0;

                    for (int i = 0; i < size; i++) {
                        var d = reader.ReadByte();
                        data |= d << (i * 8);
                    }
                    var type = (ItemType)(header >> 2 & 0b11);

                    // TODO: do we need to handle long items?

                    switch (type) {
                    case ItemType.Main:
                        var itemType = (MainItems)(header >> 4);
                        switch (itemType) {
                        case MainItems.Input:
                            collection.Add(new InputReport(data, descriptor));
                            break;
                        case MainItems.Output:
                            collection.Add(new OutputReport(data, descriptor));
                            break;
                        case MainItems.Feature:
                            collection.Add(new FeatureReport(data, descriptor));
                            break;
                        case MainItems.Collection:
                            collection = new ReportCollection(data, descriptor.UsagePage ?? 0, descriptor.Usage ?? 0, collection);
                            break;
                        case MainItems.EndCollection:
                            if (collection.Parent == null) {
                                // we have reached the end of a top-level collection
                                if (collection.Type != CollectionType.Application) {
                                    throw new InvalidDataException("Top-level collection must be Application");
                                }
                                yield return collection;
                            }
                            collection = collection.Parent;
                            break;
                        }
                        descriptor.ClearLocals();
                        break;
                    case ItemType.Global:
                    case ItemType.Local:
                        switch (descriptor.Update(header, data)) {
                        // TODO: need to accumulate Usage as array
                        case nameof(ReportDescriptorData.Push):
                            descriptorStack.Push(descriptor.Copy());
                            break;
                        case nameof(ReportDescriptorData.Pop):
                            descriptor = descriptorStack.Pop();
                            break;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                    }
                }
            }
        }

        static string Update(this ReportDescriptorData descriptor, byte header, int data)
        {
            var type = descriptor.GetType();
            var field = type.GetFields().Single(p => p.GetCustomAttribute<TagAttribute>().Match(header));
            field.SetValue(descriptor, data);
            return field.Name;
        }

        enum ItemType
        {
            Main,
            Global,
            Local,
            Reserved,
        }

        enum MainItems
        {
            Input = 0b1000,
            Output = 0b1001,
            Feature = 0b1011,
            Collection = 0b1010,
            EndCollection = 0b1100,
        }
    }
}
