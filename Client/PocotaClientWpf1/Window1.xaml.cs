using System.Windows;

namespace Net.Leksi.Pocota.Client
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private static int _hashGen = 0;
        private readonly int _hash;
        public Window1()
        {
            _hash = Interlocked.Increment(ref _hashGen);
            InitializeComponent();
        }
        public override string ToString()
        {
            return $"{GetType().Name}@{_hash}";
        }
    }
}
