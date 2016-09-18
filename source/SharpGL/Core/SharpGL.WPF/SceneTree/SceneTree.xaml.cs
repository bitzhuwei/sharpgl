using SharpGL.SceneGraph;
using System.Windows;
using System.Windows.Controls;

namespace SharpGL.WPF.SceneTree
{
    /// <summary>
    /// Interaction logic for SceneTree.xaml
    /// </summary>
    public partial class SceneTree : UserControl
    {
        public SceneTree()
        {
            InitializeComponent();

            if (Apex.Design.DesignTime.IsDesignTime)
            {
                //  Initialise the scene.
                Scene = new SceneGraph.Scene();
                SceneGraph.Helpers.SceneHelper.InitialiseModelingScene(Scene);
            }
        }

        private static readonly DependencyProperty SceneProperty =
          DependencyProperty.Register("Scene", typeof(Scene), typeof(SceneTree),
          new PropertyMetadata(null, new PropertyChangedCallback(OnSceneChanged)));

        public Scene Scene
        {
            get { return (Scene)GetValue(SceneProperty); }
            set { SetValue(SceneProperty, value); }
        }

        private static void OnSceneChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            SceneTree me = o as SceneTree;
        }
    }
}