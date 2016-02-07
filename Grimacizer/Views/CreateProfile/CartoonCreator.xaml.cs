using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.DAL.Tables;
using Grimacizer7.Utils;
using Microsoft.FaceSdk.Cartoon;
using Microsoft.FaceSdk.Common;
using Microsoft.FaceSdk.Image;
using Microsoft.FaceSdk.ImageHelper;
using Microsoft.Phone.Shell;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.CreateProfile
{
    public partial class CartoonCreator : NotifyPhoneApplicationPage
    {
        private Image<Byte> _sketchI;
        private SexType _sex;
        private HumanRaceType _race;
        private int width, height;
        private bool editProfile = false;

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged(() => Message);
                }
            }
        }
        private bool _isCalculating;
        public bool IsCalculating
        {
            get
            {
                return _isCalculating;
            }
            set
            {
                if (_isCalculating != value)
                {
                    _isCalculating = value;
                    NotifyPropertyChanged(() => IsCalculating);
                }
            }
        }
        private WriteableBitmap _imageSource;
        public WriteableBitmap ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    NotifyPropertyChanged(() => ImageSource);
                }
            }
        }

        public CartoonCreator()
        {
            InitializeComponent();
            DataContext = this;
            IsCalculating = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;
            if (NavigationContext.QueryString.ContainsKey("editprofile"))
                editProfile = true;

            Message = "Generate a random cartoon:";

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();
                width = settings.DefaultImagePixelWidth;
                height = settings.DefaultImagePixelHeight;
                _sex = settings.Sex;
                _race = settings.Race;
            }

            ImageSource = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_FACE_PHOTO, width, height);
            GenerateRandomCartoon();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            GenerateRandomCartoon();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            LocalImagesHelper.WriteImageToIsolatedStorage(Constants.DEFAULT_CARTOON_PHOTO, ImageSource);

            if (editProfile)
            {
                NavigationService.GoBack();
                return;
            }

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();
                settings.Initialized = true;
                db.SubmitChanges();
            }
            NavigationService.Navigate(new Uri(Pages.MainPage, UriKind.RelativeOrAbsolute));
        }

        private void GenerateRandomCartoon()
        {
            DisableAppBarButtons();
            Message = "Running ...";
            IsCalculating = true;

            var defaultPhoto = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_FACE_PHOTO, width, height);
            var sdkImg = ImageConverter.SystemToSdk(defaultPhoto);
            var engine = CtnEngine.Create();

            var generatorType = GetGeneratorGuid();
            var generator = engine.GetSketchGenerator(generatorType);

            ThreadPool.QueueUserWorkItem((WaitCallback)((o) =>
            {
                var random = new Random(DateTime.Now.Millisecond);

                var sketch = CtnEngine.CreateSketch();
                sketch.Image = sdkImg;

                var detector = engine.GetFaceDetector();
                var alignmentor = engine.GetFaceAlignmentor();

                Dispatcher.BeginInvoke(() => Message = "Detecting face ...");
                var rect = detector.Detect(sketch);
                detector.FinalizeRect(sketch, rect);

                alignmentor.LoadRawSketch(sketch);
                sketch = alignmentor.GetSketch();

                Dispatcher.BeginInvoke(() => Message = "Generating sketch ...");
                var rcBound = new Rectangle(0, Constants.CartoonWidth, 0, Constants.CartoonHeight);
                generator.Generate(sketch, rcBound);

                Dispatcher.BeginInvoke(() => Message = "Exaggerating face ...");
                var exaggerator = engine.GetFaceExaggerator();
                exaggerator.Exaggerate(sketch, ExaggerationType.AutoAddConstr, 0.0f);

                Dispatcher.BeginInvoke(() => Message = "Applying templates ...");
                var templateFilters = engine.GetCartoonTemplates(generatorType);
                foreach (var tf in templateFilters)
                {
                    if (tf.Count == 0) continue;

                    // Skip animation templates
                    if (tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_ANIMATEBODYCLOTH ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_ANIMATEBODYCLOTH_P ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_ANIMATEEXPRESSION ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_MULTIPART_ANIMATION)
                        continue;

                    // Randomly apply certain templates
                    if (tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_EXPRESSION ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_EXPRESSION_P ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_PET ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_PET_P ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_GLASSES ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_GLASS_P ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_BACKGROUND ||
                        tf.TemplateId == CtnGuid.GUID_TEMPLATETYPE_BACKGROUND_P)
                    {
                        if (random.Next(0, 2) == 0) continue;
                    }

                    int index = random.Next(0, tf.Count);
                    tf.Apply(sketch, index);
                }

                Dispatcher.BeginInvoke(() => Message = "Rendering sketch ...");
                float ratio, tx, ty;
                sketch.FitToRect(Rectangle.FromLTWH(0, 0, Constants.CartoonWidth, Constants.CartoonHeight), out ratio, out tx, out ty);
                _sketchI = sketch.Render(Constants.CartoonWidth, Constants.CartoonHeight);

                Dispatcher.BeginInvoke(() =>
                {
                    Message = "A random cartoon effect was generated!";
                    ImageSource = ImageConverter.SdkToSystem(_sketchI);

                    EnableAppBarButtons();
                    IsCalculating = false;
                });

                GC.Collect();
            }));
        }

        private Guid GetGeneratorGuid()
        {
            if (_sex == SexType.Male)
            {
                switch (_race)
                {
                    case HumanRaceType.Caucasian: return CtnGuid.GUID_GENERATOR_CAUCASIAN_MALE_VECTOR;
                    case HumanRaceType.African: return CtnGuid.GUID_GENERATOR_AFRICA_MALE_VECTOR;
                    case HumanRaceType.Asian: return CtnGuid.GUID_GENERATOR_ASIAN_MALE_VECTOR;
                }
            }
            else
            {
                switch (_race)
                {
                    case HumanRaceType.Caucasian: return CtnGuid.GUID_GENERATOR_CAUCASIAN_FEMALE_VECTOR;
                    case HumanRaceType.African: return CtnGuid.GUID_GENERATOR_AFRICA_FEMALE_VECTOR;
                    case HumanRaceType.Asian: return CtnGuid.GUID_GENERATOR_ASIAN_FEMALE_VECTOR;
                }
            }

            return CtnGuid.GUID_GENERATOR_CAUCASIAN_MALE_VECTOR;
        }

        private void EnableAppBarButtons()
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void DisableAppBarButtons()
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
        }
    }
}