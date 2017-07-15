﻿using LibAtem.Common;
using LibAtem.Serialization;

namespace LibAtem.Commands.MixEffects
{
    [CommandName("DCut", 4)]
    public class MixEffectCutCommand : SerializableCommandBase
    {
        [CommandId]
        [Serialize(0), Enum8]
        public MixEffectBlockId Index { get; set; }
    }
}
