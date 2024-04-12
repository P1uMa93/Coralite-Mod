﻿using Coralite.Core.Loaders;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairySpawnCondition(int fairyType)
    {
        public int fairyType = fairyType;

        public List<Condition> conditions;
        public List<(LocalizedText, Func<bool>)> extraConditions;

        public Fairy SpawnFairy() => FairyLoader.GetFairy(fairyType).NewInstance();

        /// <summary>
        /// 检测所有的Condition，如果有一个返回false那么就直接返回
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition()
        {
            if (conditions!=null)
            {
                foreach (var condition in conditions)
                    if (!condition.Predicate())
                        return false;
            }

            if (extraConditions!=null)
            {
                foreach (var condition in extraConditions)
                    if (!condition.Item2())
                        return false;
            }

            return true;
        }


        public static FairySpawnCondition CreateCondition(int fairyType)
        {
            return new FairySpawnCondition(fairyType);
        }

        /// <summary>
        /// 加条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public FairySpawnCondition AddCondition(params Condition[] conditions)
        {
            this.conditions ??= new List<Condition>();
            this.conditions.AddRange(conditions);

            return this;
        }

        /// <summary>
        /// 加条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public FairySpawnCondition AddCondition(Condition condition)
        {
            conditions ??= new List<Condition>();
            conditions.Add(condition);

            return this;
        }

        /// <summary>
        /// 加入特殊条件，可以自定义
        /// </summary>
        /// <param name="description"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public FairySpawnCondition AddCondition(LocalizedText description, Func<bool> condition)
        {
            extraConditions ??= new  List<(LocalizedText, Func<bool>)>();
            extraConditions.Add((description,condition));

            return this;
        }
    }
}
