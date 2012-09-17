using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TobbyBingMaps.Common.Entities
{
    //Class must be partial as the server componant has additional properties not required on client.
    // ReSharper disable PartialTypeWithSinglePart
    public partial class LayerDefinition : INotifyPropertyChanged
    // ReSharper restore PartialTypeWithSinglePart
    {
        private DateTime currentVersion;
        private bool isEditable;
        private bool labelOn;
        private string layerAlias;
        private string layerId;
        private string layerStyleName;
        private int layerTimeout;
        private int layerType;
        private int maxDisplayLevel;
        private byte[] mbr;
        private int minDisplayLevel;
        private bool permissionToEdit;
        private bool selected;
        private string tags;
        private int zIndex;
        private bool temporal;
        private string iconUri;
        private string layerUri;
        private int publishMethod;
        private StyleSpecification style;

        public LayerDefinition()
        {
            ObjectAttributes = new Dictionary<int, LayerObjectAttributeDefinition>();
        }

        public string LayerID
        {
            get { return layerId; }
            set
            {
                layerId = value;
                UpdateProperty("LayerID");
            }
        }

        public string LayerAlias
        {
            get { return layerAlias; }
            set
            {
                layerAlias = value;
                UpdateProperty("LayerAlias");
            }
        }

        public string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                UpdateProperty("Tags");
            }
        }

        public int LayerType
        {
            get { return layerType; }
            set
            {
                layerType = value;
                UpdateProperty("LayerType");
            }
        }

        public bool IsEditable
        {
            get { return isEditable; }
            set
            {
                isEditable = value;
                UpdateProperty("IsEditable");
            }
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                UpdateProperty("Selected");
            }
        }

        public bool LabelOn
        {
            get { return labelOn; }
            set
            {
                labelOn = value;
                UpdateProperty("LabelOn");
            }
        }

        public DateTime CurrentVersion
        {
            get { return currentVersion; }
            set
            {
                currentVersion = value;
                UpdateProperty("CurrentVersion");
            }
        }

        public int MinDisplayLevel
        {
            get { return minDisplayLevel; }
            set
            {
                minDisplayLevel = value;
                UpdateProperty("MinDisplayLevel");
            }
        }

        public int MaxDisplayLevel
        {
            get { return maxDisplayLevel; }
            set
            {
                maxDisplayLevel = value;
                UpdateProperty("MaxDisplayLevel");
            }
        }

        public byte[] MBR
        {
            get { return mbr; }
            set
            {
                mbr = value;
                UpdateProperty("MBR");
            }
        }

        public string LayerStyleName
        {
            get { return layerStyleName; }
            set
            {
                layerStyleName = value;
                UpdateProperty("LayerStyleName");
            }
        }

        public int LayerTimeout
        {
            get { return layerTimeout; }
            set
            {
                layerTimeout = value;
                UpdateProperty("LayerTimeout");
            }
        }

        public int ZIndex
        {
            get { return zIndex; }
            set
            {
                zIndex = value;
                UpdateProperty("ZIndex");
            }
        }

        public bool PermissionToEdit
        {
            get { return permissionToEdit; }
            set
            {
                permissionToEdit = value;
                UpdateProperty("PermissionToEdit");
            }
        }

        public bool Temporal
        {
            get { return temporal; }
            set
            {
                temporal = value;
                UpdateProperty("Temporal");
            }
        }

        public string IconURI
        {
            get { return iconUri; }
            set
            {
                iconUri = value;
                UpdateProperty("IconURI");
            }
        }

        public string LayerURI
        {
            get { return layerUri; }
            set
            {
                layerUri = value;
                UpdateProperty("LayerURI");
            }
        }

        public int PublishMethod
        {
            get { return publishMethod; }
            set
            {
                publishMethod = value;
                UpdateProperty("PublishMethod");
            }
        }

        public StyleSpecification Style
        {
            get { return style; }
            set
            {
                style = value;
                UpdateProperty("Style");
            }
        }

        public Dictionary<int, LayerObjectAttributeDefinition> ObjectAttributes { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void UpdateProperty(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
