using System;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Base interface for all UIKit components
    /// </summary>
    public interface IUIKitComponent : IDisposable
    {
        /// <summary>
        /// Component ID
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Component GameObject
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// Applies theme settings to the component
        /// </summary>
        void ApplyTheme(Dictionary<string, object> theme);
        
        /// <summary>
        /// Updates component properties
        /// </summary>
        void UpdateProperties(Dictionary<string, object> properties);
        
        /// <summary>
        /// Sets the component's visibility
        /// </summary>
        void SetVisible(bool visible);
        
        /// <summary>
        /// Gets the component's visibility
        /// </summary>
        bool IsVisible { get; }
    }
    
    /// <summary>
    /// Interface for components that can load assets
    /// </summary>
    public interface IAssetComponent
    {
        /// <summary>
        /// Sets the asset texture
        /// </summary>
        void SetAsset(Texture2D texture);
        
        /// <summary>
        /// Gets the current asset texture
        /// </summary>
        Texture2D GetAsset();
    }
    
    /// <summary>
    /// Interface for interactive components
    /// </summary>
    public interface IInteractableComponent
    {
        /// <summary>
        /// Event fired when the component is interacted with
        /// </summary>
        event Action<Models.ComponentAction> OnAction;
        
        /// <summary>
        /// Sets the component's interactable state
        /// </summary>
        void SetInteractable(bool interactable);
        
        /// <summary>
        /// Gets the component's interactable state
        /// </summary>
        bool IsInteractable { get; }
    }
    
    /// <summary>
    /// Interface for container components
    /// </summary>
    public interface IContainerComponent
    {
        /// <summary>
        /// Adds a child component
        /// </summary>
        void AddChild(IUIKitComponent child);
        
        /// <summary>
        /// Removes a child component
        /// </summary>
        void RemoveChild(IUIKitComponent child);
        
        /// <summary>
        /// Gets all child components
        /// </summary>
        IUIKitComponent[] GetChildren();
        
        /// <summary>
        /// Clears all children
        /// </summary>
        void ClearChildren();
    }
    
    /// <summary>
    /// Interface for text-based components
    /// </summary>
    public interface ITextComponent
    {
        /// <summary>
        /// Sets the text content
        /// </summary>
        void SetText(string text);
        
        /// <summary>
        /// Gets the text content
        /// </summary>
        string GetText();
        
        /// <summary>
        /// Sets the text color
        /// </summary>
        void SetTextColor(Color color);
        
        /// <summary>
        /// Sets the font size
        /// </summary>
        void SetFontSize(float size);
    }
}