using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

namespace jigsaw;

public partial class MainPage : ContentPage
{
    private readonly List<Image> _images;
    private readonly List<Frame> _frames;
    private Frame _selectedFrame;

    public MainPage()
    {
        InitializeComponent();

        const string imageName = "monalisa";

        this._frames = ImageGrid.Children.OfType<Frame>().ToList();
        this._images = _frames.Select(f => f.Content).OfType<Image>().ToList();

        Enumerable.Range(0,9).ForEach(i =>_images[i].Source = ImageSource.FromFile($"{imageName}_{9 - i}.png"));

        _frames.ForEach(frame =>
        {
            frame.GestureRecognizers.Add(new TapGestureRecognizer(RotateFrame) {NumberOfTapsRequired = 2});
            frame.GestureRecognizers.Add(new TapGestureRecognizer(SelectFrame) {NumberOfTapsRequired = 1});
        });
        
        ScrambleFrames();
    }

    private void RotateFrame(View view, object o)
    {
        if (view is not Frame {Content: Image image} frame)
            return;

        frame.Rotation += 90;
        CheckWin();
    }

    private void SelectFrame(View view, object o)
    {
        if (view is not Frame {Content: Image image} frame)
            return;

        if (_selectedFrame == frame)
        {
            frame.BorderColor = Color.Default;
            _selectedFrame = null;
            return;
        }

        if (_selectedFrame != null)
        {
            SwapFrames(frame, _selectedFrame);
            _selectedFrame.BorderColor = Color.Default;
            frame.BorderColor = Color.Default;
            _selectedFrame = null;
            return;
        }

        frame.BorderColor = Color.Red;
        _selectedFrame = frame;
    }

    private void SwapFrames(Frame targetFrame, Frame sourceFrame)
    {
        var targetImage = targetFrame.Content as Image;
        var sourceImage = sourceFrame.Content as Image;
        
        var sourceRotation = sourceFrame.Rotation;
        var targetRotation = targetFrame.Rotation;

        targetFrame.Content = sourceImage;
        sourceFrame.Content = targetImage;

        sourceFrame.Rotation = targetRotation;
        targetFrame.Rotation = sourceRotation;

        CheckWin();
    }

    private void CheckWin()
    {

        //check current order of frames against original order
        var images = _frames.Select(frame => frame.Content).OfType<Image>().ToList();
        var correctOrder = images.SequenceEqual(_images);
        //check if any frames are rotated
        var correctRotation = _frames.All(frame => frame.Rotation % 360 == 0);
        
        if (correctOrder && correctRotation)
        {
            DisplayAlert("You win!", "Congratulations, You solved the puzzle!", "OK");
            ScrambleFrames();
        }
    }

    private void ScrambleFrames()
    {
        var random = new Random();
        _frames.ForEach(frame =>
        {
            frame.Rotation = random.Next(0, 4) * 90;
            SwapFrames(frame, _frames[random.Next(0, 9)]);
        });

    }
}