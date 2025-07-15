namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Predefined layout sizes for UIKit templates
    /// </summary>
    public enum LayoutSize
    {
        /// <summary>
        /// Takes the entire screen
        /// </summary>
        FullScreen,
        
        /// <summary>
        /// Large modal: 90% width, 80% height
        /// </summary>
        LargeModal,
        
        /// <summary>
        /// Medium modal: 80% width, 60% height
        /// </summary>
        MediumModal,
        
        /// <summary>
        /// Small modal: 70% width, 40% height
        /// </summary>
        SmallModal,
        
        /// <summary>
        /// Top banner: 100% width, 15% height
        /// </summary>
        TopBanner,
        
        /// <summary>
        /// Bottom sheet: 100% width, 50% height
        /// </summary>
        BottomSheet,
        
        /// <summary>
        /// Center square: 80% of min(width, height)
        /// </summary>
        CenterSquare,
        
        /// <summary>
        /// Floating button: Fixed 80x80 dp
        /// </summary>
        FloatingButton,
        
        /// <summary>
        /// Side panel: 30% width, 100% height (for tablets)
        /// </summary>
        SidePanel
    }
    
    /// <summary>
    /// Component size constraints for consistent UI
    /// </summary>
    public enum ComponentSize
    {
        /// <summary>
        /// Tiny: 32x32 for images, 12pt for text
        /// </summary>
        Tiny,
        
        /// <summary>
        /// Small: 64x64 for images, 14pt for text
        /// </summary>
        Small,
        
        /// <summary>
        /// Medium: 128x128 for images, 16pt for text
        /// </summary>
        Medium,
        
        /// <summary>
        /// Large: 256x256 for images, 20pt for text
        /// </summary>
        Large,
        
        /// <summary>
        /// Extra large: 384x384 for images, 24pt for text
        /// </summary>
        ExtraLarge,
        
        /// <summary>
        /// Hero: 16:9 aspect ratio, responsive to container
        /// </summary>
        Hero,
        
        /// <summary>
        /// Banner: 3:1 aspect ratio, responsive to container
        /// </summary>
        Banner,
        
        /// <summary>
        /// Fill: Fills the available space
        /// </summary>
        Fill
    }
    
    /// <summary>
    /// Supported component types in UIKit
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        /// Image component with remote loading support
        /// </summary>
        Image,
        
        /// <summary>
        /// Text component with styling
        /// </summary>
        Text,
        
        /// <summary>
        /// Interactive button component
        /// </summary>
        Button,
        
        /// <summary>
        /// Container for grouping components
        /// </summary>
        Container,
        
        /// <summary>
        /// Countdown timer component
        /// </summary>
        Timer,
        
        /// <summary>
        /// Progress bar component
        /// </summary>
        ProgressBar,
        
        /// <summary>
        /// Video player component
        /// </summary>
        Video,
        
        /// <summary>
        /// Grid layout container
        /// </summary>
        Grid,
        
        /// <summary>
        /// List/scroll view container
        /// </summary>
        List,
        
        /// <summary>
        /// Input field component
        /// </summary>
        Input,
        
        /// <summary>
        /// Toggle/checkbox component
        /// </summary>
        Toggle,
        
        /// <summary>
        /// Slider component
        /// </summary>
        Slider,
        
        /// <summary>
        /// Spacer for layout
        /// </summary>
        Spacer
    }
    
    /// <summary>
    /// Action types for component interactions
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// No action
        /// </summary>
        None,
        
        /// <summary>
        /// Close the template
        /// </summary>
        Close,
        
        /// <summary>
        /// Open a URL
        /// </summary>
        OpenUrl,
        
        /// <summary>
        /// Send an event to the game
        /// </summary>
        SendEvent,
        
        /// <summary>
        /// Navigate to another template
        /// </summary>
        Navigate,
        
        /// <summary>
        /// Make a purchase
        /// </summary>
        Purchase,
        
        /// <summary>
        /// Share content
        /// </summary>
        Share,
        
        /// <summary>
        /// Play a sound
        /// </summary>
        PlaySound,
        
        /// <summary>
        /// Custom action defined by the game
        /// </summary>
        Custom
    }
    
    /// <summary>
    /// Animation types for showing/hiding templates
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// No animation
        /// </summary>
        None,
        
        /// <summary>
        /// Fade in/out
        /// </summary>
        Fade,
        
        /// <summary>
        /// Slide from direction
        /// </summary>
        Slide,
        
        /// <summary>
        /// Scale up/down
        /// </summary>
        Scale,
        
        /// <summary>
        /// Bounce effect
        /// </summary>
        Bounce,
        
        /// <summary>
        /// Flip animation
        /// </summary>
        Flip,
        
        /// <summary>
        /// Custom animation
        /// </summary>
        Custom
    }
    
    /// <summary>
    /// Alignment options for components
    /// </summary>
    public enum Alignment
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}