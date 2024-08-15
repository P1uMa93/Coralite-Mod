﻿using Coralite.Helpers;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class UpgradeableLinerSender : MagikeLinerSender, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableContainer : MagikeContainer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableActiveProducer : MagikeActiveProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableExtractProducer : MagikeExtractProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public abstract class UpgradeableBiomeProducer : BiomeProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }
}
