using System;
using System.Configuration;

namespace auth
{
    public class Config : ConfigurationSection
    {
        private static Config _config = (Config)ConfigurationManager.GetSection("MyConfig");
        public static Config Settings { get { return _config; }}

        [ConfigurationProperty("NAS_IDENTIFIER", IsRequired = false)]
        public string NAS_IDENTIFIER { get { return (string)base["NAS_IDENTIFIER"]; } }

        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers { get { return (ServerElementCollection)base["servers"]; } }

    }

    public class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get { return (string)base["name"]; }}

        [ConfigurationProperty("sharedsecret", IsRequired = true)]
        public string sharedsecret { get { return (string)base["sharedsecret"]; } }

        [ConfigurationProperty("authport", DefaultValue = (uint) 1812, IsRequired = false)]
        public uint authport { get { return (uint)base["authport"]; } }

        [ConfigurationProperty("retries", DefaultValue = 1, IsRequired = false)]
        public int retries { get { return (int)base["retries"]; } }

        [ConfigurationProperty("wait", DefaultValue = 5, IsRequired = false)]
        public int wait { get { return (int)base["wait"]; } }

        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers { get { return (ServerElementCollection)base["servers"]; } }

    }
    
    [ConfigurationCollection(typeof(ServerElement), AddItemName = "server", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ServerElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        { get { return ConfigurationElementCollectionType.BasicMap; } }

        protected override string ElementName
        { get { return "server"; } }

        protected override ConfigurationElement CreateNewElement()
            {return new ServerElement();}

        protected override object GetElementKey(ConfigurationElement element)
            {return (element as ServerElement).Name;}

        public ServerElement this[int index]
        {
            get { return (ServerElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        new public ServerElement this[string name]
        { get { return (ServerElement)base.BaseGet(name); }}
    }

}
