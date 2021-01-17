// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubEvent.cs" company="Watson Laboratory">
//   Copyright © 2021 Watson Laboratory
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace home_automation_hub
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class HubEvent
    {
        [JsonProperty(PropertyName = "descriptionText")]
        public string DescriptionText { get; set; }

        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        public bool IsActiveEvent => this.Name == "active";
        public bool IsButtonHeldEvent => this.Name == "held";
        public bool IsButtonPushedEvent => this.Name == "pushed";
        public bool IsClosedEvent => this.Value == "closed";
        public bool IsInactiveEvent => this.Name == "inactive";
        public bool IsLockedEvent => this.Value == "locked";
        public bool IsOffEvent => this.Value == "off";
        public bool IsOnEvent => this.Value == "on";

        public bool IsOpenEvent => this.Value == "open";
        public bool IsUnLockedEvent => this.Value == "unlocked";

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        public Dictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>
            {
                { "Device ID", this.DeviceId },
                { "Description", this.DescriptionText },
                { "Display Name", this.DisplayName },
                { "Name", this.Name },
                { "Source", this.Source },
                { "Value", this.Value }
            };
        }
    }
}