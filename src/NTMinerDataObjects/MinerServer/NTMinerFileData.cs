﻿using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class NTMinerFileData : INTMinerFile, IDbEntity<Guid>, IGetSignData {
        public NTMinerFileData() {

        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public NTMinerAppType AppType { get; set; }

        public string FileName { get; set; }

        public string Version { get; set; }

        public string VersionTag { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime PublishOn { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(FileName)).Append(FileName)
                .Append(nameof(Version)).Append(Version)
                .Append(nameof(VersionTag)).Append(VersionTag)
                .Append(nameof(CreatedOn)).Append(CreatedOn.ToUlong())
                .Append(nameof(PublishOn)).Append(PublishOn.ToUlong())
                .Append(nameof(Title)).Append(Title)
                .Append(nameof(Description)).Append(Description);
            return sb;
        }

        public Version GetVersion() {
            if (string.IsNullOrEmpty(Version)) {
                return new Version(1, 0);
            }
            if (System.Version.TryParse(this.Version, out Version v)) {
                return v;
            }
            return new Version(1, 0);
        }
    }
}
