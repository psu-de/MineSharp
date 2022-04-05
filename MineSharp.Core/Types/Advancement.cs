using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class Advancement {

        public bool HasParent { get; private set; }
        public Identifier? ParentId { get; private set; }
        public bool HasDisplay { get; private set; }
        public Advancement.Display? DisplayData { get; private set; }
        public Identifier[] Criteria { get; private set; }
        public string[][] Requirements { get; private set; }

        public Advancement(bool hasParent, Identifier? parentId, bool hasDisplay, Display? displayData, Identifier[] criteria, string[][] requirements) {
            HasParent = hasParent;
            ParentId = parentId;
            HasDisplay = hasDisplay;
            DisplayData = displayData;
            Criteria = criteria;
            Requirements = requirements;
        }

        public class Display {
            public Chat Title { get; private set; }
            public Chat Description { get; private set; }
            public Slot Icon { get; private set; }
            public FrameType Type { get; private set; }
            public int Flags { get; private set; }
            public Identifier? BackgroundTexture { get; private set; }
            public float XCoord { get; private set; }
            public float YCoord { get; private set; }

            public Display(Chat title, Chat description, Slot icon, FrameType type, int flags, Identifier? backgroundTexture, float xCoord, float yCoord) {
                Title = title;
                Description = description;
                Icon = icon;
                Type = type;
                Flags = flags;
                BackgroundTexture = backgroundTexture;
                XCoord = xCoord;
                YCoord = yCoord;
            }

            public enum FrameType {
                Task = 0,
                Challenge = 1,
                Goal = 2
            }
        }
    }
}
