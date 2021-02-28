using System.Collections.Generic;
using System.Reflection;
using Server.Commands;
using Server.HuePickers;
using Server.Network;

namespace Server.Gumps
{
    public class SetGump : Gump
    {
        public static readonly bool OldStyle = PropsConfig.OldStyle;

        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;

        public static readonly int SetWidth = PropsConfig.SetWidth;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

        public static readonly int OffsetSize = PropsConfig.OffsetSize;

        public static readonly int EntryHeight = PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;

        private static readonly int EntryWidth = 212;

        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + 2 * (EntryHeight + OffsetSize);

        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly List<object> m_List;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly int m_Page;
        private readonly PropertyInfo m_Property;
        private readonly Stack<StackEntry> m_Stack;
        private bool m_Edit = false;
     
        public SetGump(
            PropertyInfo prop, Mobile mobile, object o, Stack<StackEntry> stack, int page, List<object> list,bool IsEdit=false
        ) : base(
            GumpOffsetX,
            GumpOffsetY
        )
        {
            m_Edit = IsEdit;
            m_Property = prop;
            m_Mobile = mobile;
            m_Object = o;
            m_Stack = stack;
            m_Page = page;
            m_List = list;

            var canNull = !prop.PropertyType.IsValueType;
            var canDye = prop.IsDefined(typeof(HueAttribute), false);
            var isBody = prop.IsDefined(typeof(BodyAttribute), false);

            var val = prop.GetValue(m_Object, null);
            var initialText = val switch
            {
                null                      => "",
                TextDefinition definition => definition.GetValue(),
                _                         => val.ToString()
            };

            AddPage(0);

            AddBackground(
                0,
                0,
                BackWidth,
                BackHeight + (canNull ? EntryHeight + OffsetSize : 0) + (canDye ? EntryHeight + OffsetSize : 0) +
                (isBody ? EntryHeight + OffsetSize : 0),
                BackGumpID
            );
            AddImageTiled(
                BorderSize,
                BorderSize,
                TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0),
                TotalHeight + (canNull ? EntryHeight + OffsetSize : 0) + (canDye ? EntryHeight + OffsetSize : 0) +
                (isBody ? EntryHeight + OffsetSize : 0),
                OffsetGumpID
            );

            var x = BorderSize + OffsetSize;
            var y = BorderSize + OffsetSize;

            AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, prop.Name);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
            {
                AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            }

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            AddTextEntry(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, 0, initialText);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
            {
                AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            }

            AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1);

            if (canNull)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Null");
                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                {
                    AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
                }

                AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2);
            }

            if (canDye)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Hue Picker");
                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                {
                    AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
                }

                AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3);
            }

            if (isBody)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Body Picker");
                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                {
                    AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
                }

                AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 4);
            }
        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            object toSet;
            bool shouldSet, shouldSend = true;

            switch (info.ButtonID)
            {
                case 1:
                    {
                        var text = info.GetTextEntry(0);

                        if (text != null)
                        {
                            try
                            {
                                toSet = PropertiesGump.GetObjectFromString(m_Property.PropertyType, text.Text);
                                shouldSet = true;
                            }
                            catch
                            {
                                toSet = null;
                                shouldSet = false;
                                m_Mobile.SendMessage("Bad format");
                            }
                        }
                        else
                        {
                            toSet = null;
                            shouldSet = false;
                        }

                        break;
                    }
                case 2: // Null
                    {
                        toSet = null;
                        shouldSet = true;

                        break;
                    }
                case 3: // Hue Picker
                    {
                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;

                        m_Mobile.SendHuePicker(new InternalPicker(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List));

                        break;
                    }
                case 4: // Body Picker
                    {
                        toSet = null;
                        shouldSet = false;
                        shouldSend = false;

                        m_Mobile.SendGump(new SetBodyGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List));

                        break;
                    }
                default:
                    {
                        toSet = null;
                        shouldSet = false;

                        break;
                    }
            }

            if (shouldSet)
            {
                try
                {
                    CommandLogging.LogChangeProperty(
                        m_Mobile,
                        m_Object,
                        m_Property.Name,
                        toSet?.ToString() ?? "(null)"
                    );
                    m_Property.SetValue(m_Object, toSet, null);
                    PropertiesGump.OnValueChanged(m_Object, m_Property, m_Stack);
                }
                catch
                {
                    m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }
            }

            if (shouldSend)
            {
                if(m_Edit)
                {
                    m_Mobile.SendGump(new GlobalPropsGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
                }
                else m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
               
            }
        }

        private class InternalPicker : HuePicker
        {
            private readonly List<object> m_List;
            private readonly Mobile m_Mobile;
            private readonly object m_Object;
            private readonly int m_Page;
            private readonly PropertyInfo m_Property;
            private readonly Stack<StackEntry> m_Stack;

            public InternalPicker(
                PropertyInfo prop, Mobile mobile, object o, Stack<StackEntry> stack, int page,
                List<object> list
            ) : base(((IHued)o).HuedItemID)
            {
                m_Property = prop;
                m_Mobile = mobile;
                m_Object = o;
                m_Stack = stack;
                m_Page = page;
                m_List = list;
            }

            public override void OnResponse(int hue)
            {
                try
                {
                    CommandLogging.LogChangeProperty(m_Mobile, m_Object, m_Property.Name, hue.ToString());
                    m_Property.SetValue(m_Object, hue, null);
                    PropertiesGump.OnValueChanged(m_Object, m_Property, m_Stack);
                }
                catch
                {
                    m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }

                m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
            }
        }
    }
}
