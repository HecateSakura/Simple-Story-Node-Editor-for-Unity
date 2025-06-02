#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;
namespace Rift.Story
{
    public class BasicNode : Node
    {
        public Action<Guid> OnNodeSelected;
        public Action<Guid> OnNodeUnselected;
        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeUnselected?.Invoke(new Guid(viewDataKey));
        }
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(new Guid(viewDataKey));
        }
    }
}
#endif