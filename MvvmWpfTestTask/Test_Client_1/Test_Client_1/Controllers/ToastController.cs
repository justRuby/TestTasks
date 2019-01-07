using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Test_Client_1.Controllers
{
    using Test_Client_1.Models;
    using Test_Client_1.CustomControls;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ToastController
    {
        private const int _timeElapse = 2700;

        private static ToastController _instance;
        private static object syncRoot = new Object();

        private AnimatedToastBlock _current = null;
        private Queue<ToastModel> _queue = new Queue<ToastModel>();

        private DispatcherTimer _toastTimer = new DispatcherTimer();

        private Grid _parentGrid;
        private ToastModel ToastModel { get; set; }

        private ToastController() { }

        public static ToastController GetInstance()
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new ToastController();
                }
            }
            return _instance;
        }

        public void InitializeComponents(Grid parent)
        {
            _toastTimer.Interval = TimeSpan.FromMilliseconds(_timeElapse);
            _toastTimer.Tick += _toastTimer_Tick;

            _parentGrid = parent;
            ToastModel = new ToastModel(text: "BASE", 
                                        fontColor: Color.FromArgb(255, 255, 255, 255), 
                                        backgroundColor: Color.FromArgb(255, 56, 56, 56));
        }

        public async Task<bool> ViewToast(string text)
        {
            ToastModel.Text = text;
            _queue.Enqueue(ToastModel);

            if (_queue.Count == 1)
                _toastTimer.Start();

            return await Task.FromResult(true);
        }

        private void _toastTimer_Tick(object sender, EventArgs e)
        {
            if(_queue.Count > 0)
            {

                if (_current != null)
                {
                    _parentGrid.Children.Remove(_current);
                    _current = null;
                }

                _current = new AnimatedToastBlock(_queue.Dequeue());
                _parentGrid.Children.Add(_current);

                _current.StartAnimation();

            }
            else
            {
                _toastTimer.Stop();
            }
        }
    }
}
