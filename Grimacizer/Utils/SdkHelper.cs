//#define CALL_ADVANCED_DETECTION

using Microsoft.FaceSdk.Alignment;
using Microsoft.FaceSdk.Common;
using Microsoft.FaceSdk.Detection;
using Microsoft.FaceSdk.Image;
using Microsoft.FaceSdk.ImageHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Grimacizer7.Utils
{
    /// <summary>
    /// Detector wrapper
    /// </summary>
    public class DetectorWrapper
    {
        private FaceRect[] faces;
        private IFaceDetector detector;

        public DetectorWrapper(FaceDetectionType type)
        {
            this.detector = FaceDetectorFactory.Create(type);
            this.Type = type;
        }

        /// <summary>
        /// Detect faces in the given image
        /// </summary>
        /// <param name="image">Image object</param>
        public void Detect(Image<byte> image)
        {
            if (image == null) throw new ArgumentNullException("image", "Invalid parameter.");

#if CALL_ADVANCED_DETECTION
            // 
            // You can fine control the detection behavior by providing more information, like
            // whether to use color information, the face area in the image, the minimum and maximum face size,
            // whether to use post-filter (post filter can efficiently prune false positives), 
            // the search step (in pixel) between neighboring sliding windows (in both horizontal and vertical directions).
            // 
            // The following code shows how to call the advanced Detect method
            //

            // Assume the input is a color image, convert it to grayscale.
            ImageGray gray = new ImageGray(image);

            // The minimum detectable face size is given by Detector.MinResolution. Small faces below that resolution 
            // will not be included in the detection result. For convenince, we specify (0,0) here.
            //
            // The maximum detectable face size is given by Detector.MaxResolution. Large faces beyond that resolution 
            // will not be included in the detection result. For convenince, we specify image size here.
            //
            // The step ratio specifies the gaps (in pixel) between neighboring sliding windows (a sliding window defines 
            // a search area) in both horiztonal and vertical directions. For example, if the window size is 40x40, given 
            // a step ratio of 0.1, the gaps will be 4 pixels.
            // A smaller ratio probably will improve the recall, but takes more time. We use 0.1 as a default value.
            //
            // The post filter flag indicates whether to apply post filter over the raw detection result. By default, we 
            // turn on this flag to efficiently remove false positives.
            //
            // The search region parameter defines the local area to search the faces. By default, we search the entire image.
            //
            // If the color image is provided, the detector will employ the color information to search the faces. 
            // For example, it will first label the possible skin region and then search faces on those regions only, which
            // can greatly improve the detection speed.

            this.faces = this.detector.Detect(gray, Size.Empty, gray.Size, 0.1f, true, gray.Region, image); 
#else
            // If the input is grayscale image, the Detect method will search the face over the entire
            // image region based on the local patterns.
            // If the input is a color image (RGB or ARGB), internally the Detect method will
            // convert it to grayscale, and employ color information to search the face area.
            this.faces = this.detector.Detect(image);
#endif
        }

        /// <summary>
        /// Get the detector type
        /// </summary>
        public FaceDetectionType Type { get; private set; }

        /// <summary>
        /// Get the detection results
        /// </summary>
        public ReadOnlyCollection<FaceRect> Results
        {
            get
            {
                Debug.Assert(this.faces != null);
                return new ReadOnlyCollection<FaceRect>(this.faces);
            }
        }
    }

    /// <summary>
    /// Alignment wrapper
    /// </summary>
    public class AlignmentWrapper
    {
        private IFaceAlignmentor alignmentor;
        private PointF[][] points;

        public AlignmentWrapper(FaceAlignmentType type)
        {          
            this.alignmentor = FaceAlignmentorFactory.Create(type);
            this.Type = type;
        }

        /// <summary>
        /// Detect face feature points
        /// </summary>
        /// <param name="grayImage">Grayscale image</param>
        /// <param name="faces">Face collection</param>
        public void Align(Image<byte> grayImage, IEnumerable<Rectangle> faces)
        {
            if (grayImage == null) throw new ArgumentNullException("grayImage", "Invalid parameter.");
            if (faces == null) throw new ArgumentNullException("faces", "Invalid parameter.");
            if (grayImage.Channels != Color.GrayChannels) throw new InvalidOperationException("This method works only on grayscale image.");

            this.points = (from f in faces select this.alignmentor.Align(grayImage, f)).ToArray();
        }

        /// <summary>
        /// Get the alignment type
        /// </summary>
        public FaceAlignmentType Type { get; private set; }

        /// <summary>
        /// Get the alignment results
        /// </summary>
        public ReadOnlyCollection<PointF[]> Results
        {
            get
            {
                Debug.Assert(this.points != null);
                return new ReadOnlyCollection<PointF[]>(this.points);
            }
        }
    }

    /// <summary>
    /// A helper class which provides handy accesses to FaceSDK functions.
    /// </summary>
    public class SdkHelper : INotifyPropertyChanged
    {
        private Dictionary<FaceDetectionType, DetectorWrapper> detectors = new Dictionary<FaceDetectionType, DetectorWrapper>();
        private Dictionary<FaceAlignmentType, AlignmentWrapper> alignmentors = new Dictionary<FaceAlignmentType, AlignmentWrapper>();

        private FaceDetectionType detectorType;
        private FaceAlignmentType alignmentType;

        public static bool foundFace;
       
        /// <summary>
        /// For rendering
        /// </summary>
        private float radias = 1.0f;
        private Color[] colors = new Color[] {
            Color.FromArgb(255, 255, 0,   0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 0,   255),
            Color.FromArgb(255, 255,  255,  0),
            Color.FromArgb(255, 255,  0,  255),
            Color.FromArgb(255, 0,  255,  255),
            Color.FromArgb(255, 255, 126, 0),
            Color.FromArgb(255, 215,  86, 0),
            Color.FromArgb(255, 33,  79,  84),
            Color.FromArgb(255, 77,  109, 243),
        };

        /// <summary>
        /// Constructor
        /// </summary>
        public SdkHelper()
        {
            foundFace = false;
          
            this.Detectors = new FaceDetectionType[]
            {
                FaceDetectionType.Haar, 
                FaceDetectionType.Multiview,
                FaceDetectionType.MultiviewPyramid,
            };

            this.Alignmentors = new FaceAlignmentType[] 
            { 
                FaceAlignmentType.Asm, 
                FaceAlignmentType.NeuralNetwork,
            };

            this.detectorType = this.Detectors.First();
            this.alignmentType = this.Alignmentors.First();
        }

        private void InvokePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Get/set the detection algorithm
        /// </summary>
        public FaceDetectionType Detector
        {
            get { return this.detectorType; }
            set
            {
                DetectorWrapper wrapper;
                if (!this.detectors.TryGetValue(value, out wrapper))
                {
                    wrapper = new DetectorWrapper(value);
                    this.detectors[value] = wrapper;
                }

                this.detectorType = value;
                this.InvokePropertyChanged("Detector");
            }
        }

        /// <summary>
        /// Get/set the alignment algorithm
        /// </summary>
        public FaceAlignmentType Alignmentor
        {
            get { return this.alignmentType; }
            set
            {
                AlignmentWrapper wrapper;
                if (!this.alignmentors.TryGetValue(value, out wrapper))
                {
                    wrapper = new AlignmentWrapper(value);
                    this.alignmentors[value] = wrapper;
                }

                this.alignmentType = value;
                this.InvokePropertyChanged("Alignmentor");
            }
        }

        /// <summary>
        /// Get the supported detection algorithm enumerations
        /// </summary>
        public IEnumerable<FaceDetectionType> Detectors { get; private set; }

        /// <summary>
        /// Get the supported alignment algorithm enumerations
        /// </summary>
        public IEnumerable<FaceAlignmentType> Alignmentors { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Detect face, feature points
        /// </summary>
        /// <param name="image">Input image</param>
        /// <returns>The image with detection result</returns>
        /// 

        // We write the results
        public static ReadOnlyCollection<PointF[]> results;

        public Image<byte> Detect(Image<byte> image)
        {
            // Detect faces
            var dtor = this.detectors[this.detectorType];
            dtor.Detect(image);

            // Detect face feature points
            Image<byte> gray = new ImageGray(image);
            var ator = this.alignmentors[this.alignmentType];
            var faceRects = from f in dtor.Results select f.Rect;
            ator.Align(gray, faceRects);

            // Render the result
            int colorIndex = ((int)this.detectorType - 1) * this.Alignmentors.Count() + ((int)this.alignmentType - 1);
            using (var render = Renderer.Create(new ImageARGB(image)))
            {
                results = ator.Results;
                foreach (var shape in ator.Results)
                {
                    render.DrawPoints(shape, this.radias, this.colors[colorIndex]);
                    foundFace = true;
                }

                return render.Image;
            }
        }

        /// <summary>
        /// Initialize the Face SDK component
        /// </summary>
        public void Initialize()
        {
            this.detectors[this.detectorType] = new DetectorWrapper(this.detectorType);
            this.alignmentors[this.alignmentType] = new AlignmentWrapper(this.alignmentType);
        }
    }
}
