using Microsoft.CodeAnalysis.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Steelax.EasyRoslyn.Common.SourceBag;

namespace Steelax.EasyRoslyn.Common
{
    public class SourceBag: IEnumerable<SourceBagItem>
    {
        readonly List<SourceBagItem> sources = new();

        internal SourceBag() { }

        internal class SourceBagItem
        {
            public readonly SourceText SourceText;
            public readonly string Path;
            public SourceBagItem(string text, string path)
            {
                SourceText = SourceText.From(text, Encoding.UTF8);
                Path = path;
            }
            public SourceBagItem(string path)
            {
                using var stream = File.Open(path, FileMode.Open);

                SourceText = SourceText.From(stream, Encoding.UTF8, throwIfBinaryDetected: true, canBeEmbedded: true);
                Path = path;
            }
        }

        public SourceBag FromText(string text, string path)
        {
            sources.Add(new(text, path));
            return this;
        }
        public SourceBag FromFile(string path)
        {
            sources.Add(new(path));
            return this;
        }

        IEnumerator<SourceBagItem> IEnumerable<SourceBagItem>.GetEnumerator() => sources.GetEnumerator();

        public IEnumerator GetEnumerator() => GetEnumerator();
    }
}