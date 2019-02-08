﻿using Aliyun.OSS;
using NTMiner.AppSetting;
using NTMiner.Data;
using NTMiner.User;
using System;

namespace NTMiner {
    public interface IHostRoot {
        DateTime StartedOn { get; }

        IHostConfig HostConfig { get; }

        OssClient OssClient { get; }

        IUserSet UserSet { get; }
        IAppSettingSet AppSettingSet { get; }
        ICalcConfigSet CalcConfigSet { get; }
        IClientCoinSnapshotSet ClientCoinSnapshotSet { get; }
        IClientSet ClientSet { get; }
        ICoinSnapshotSet CoinSnapshotSet { get; }
        IMineWorkSet MineWorkSet { get; }
        IMinerGroupSet MinerGroupSet { get; }
        IWalletSet WalletSet { get; }
        IMineProfileManager MineProfileManager { get; }
        INTMinerFileSet NTMinerFileSet { get; }
    }
}
