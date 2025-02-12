﻿using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.ThunderChapter1
{
    public class Thunder1Knowldege : KeyKnowledge
    {
        public override int Type => KeyKnowledgeID.Thunder1;

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ThunderveinDragonPage1>();
    }
}
