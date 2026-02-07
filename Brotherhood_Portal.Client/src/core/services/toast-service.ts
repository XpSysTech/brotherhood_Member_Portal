import { Injectable } from '@angular/core';
import { ToastType } from '../types/toast-type';

/**
 * ToastService
 * 
 * Provides a reusable notification system for displaying temporary messages to users.
 * Uses Angular's dependency injection to ensure a single instance across the application.
 * Toasts are DOM-based (not component-based) for lightweight, fast rendering.
 */
@Injectable({
  providedIn: 'root'
})
export class ToastService {

  // Unique identifier for the DOM container that holds all toast notifications
  private readonly containerId = 'toast-container';

  constructor() {
    // Initialize the toast container when the service is created
    this.ensureToastContainer();
  }


  /* ------------------------------
     Toast Container
  ------------------------------ */
  /**
   * Ensures the toast container exists in the DOM.
   * Creates a fixed-position container if it doesn't already exist.
   * This container holds all individual toast notifications.
   */
  private ensureToastContainer(): void {
    // Return early if container already exists
    if (document.getElementById(this.containerId)) return;

    // Create the main container element
    const container = document.createElement('div');
    container.id = this.containerId;

    // Apply Tailwind CSS classes for positioning and layout
    // - fixed: position fixed to viewport
    // - bottom-4 right-4: place in bottom-right corner with spacing
    // - z-50: ensure it appears above other content
    // - flex flex-col gap-3: stack toasts vertically with spacing
    
    container.className = `
      fixed bottom-4 right-4 z-50
      flex flex-col gap-3
    `;

    // container.className = `
    //   fixed top-4 left-1/2 z-50 transform -translate-x-1/2
    //   flex flex-col gap-3 items-center
    // `;

    // Add the container to the document body
    document.body.appendChild(container);
  }


  /* ------------------------------
     Toast Creation
  ------------------------------ */
  /**
   * Creates and displays a toast notification with the specified message and type.
   * The toast automatically dismisses after the specified duration (default 5 seconds).
   * 
   * @param message - The text content to display in the toast
   * @param type - The toast type (success, error, warning, info) which determines styling
   * @param duration - Optional duration in milliseconds before auto-dismissal (default: 5000ms)
   */
  private createToast(
    message: string,
    type: ToastType,
    duration = 5000
  ): void {
    // Get the main toast container
    const container = document.getElementById(this.containerId);
    if (!container) return;

    // Create the individual toast element
    const toast = document.createElement('div');

    // Apply Tailwind CSS classes for styling and animation
    // - flex items-center justify-between: layout for content and close button
    // - gap-4: spacing between elements
    // - px-4 py-3: internal padding
    // - rounded-xl: rounded corners
    // - shadow-lg: drop shadow for depth
    // - text-sm font-medium: font sizing and weight
    // - animate-slide-in: animation class for entrance effect
    // - getToastStyles(type): dynamic colors based on toast type
    toast.className = `
      flex items-center justify-between gap-4
      px-4 py-4 rounded-xl shadow-lg
      text-sm font-large
      animate-slide-in
      ${this.getToastStyles(type)}
    `;

    // Set the HTML content: message text and a close button
    toast.innerHTML = `
      <span>${message}</span>
      <button
        aria-label="Close notification"
        class="text-lg leading-none hover:opacity-70"
      >
        ×
      </button>
    `;

    // Attach click handler to the close button for manual dismissal
    toast.querySelector('button')?.addEventListener('click', () => {
      this.removeToast(toast);
    });

    // Add the toast to the container for display
    container.appendChild(toast);

    // Automatically remove the toast after the specified duration
    setTimeout(() => this.removeToast(toast), duration);
  }

  /**
   * Removes a toast notification from the DOM with a fade-out animation.
   * 
   * @param toast - The toast element to remove
   */
  private removeToast(toast: HTMLElement): void {
    // Add opacity and transition classes for fade-out animation
    toast.classList.add('opacity-0', 'transition-opacity', 'duration-300');

    // Remove the element from DOM after animation completes
    setTimeout(() => {
      toast.remove();
    }, 300);
  }


  /* ------------------------------
     Styling Map
  ------------------------------ */
  /**
   * Returns the appropriate Tailwind CSS classes for the given toast type.
   * This controls the background color and text color of the toast notification.
   * 
   * @param type - The toast type (success, error, warning, info)
   * @returns CSS class string for the specified type
   */
  private getToastStyles(type: ToastType): string {
    switch (type) {
      case 'success':
        // Green background for positive/successful actions
        // return 'bg-white text-green-600 border border-green-600';
        return 'bg-green-600 text-white';
      case 'error':
        // Red background for errors or failures
        return 'bg-red-600 text-white';
      case 'warning':
        // Yellow background for warnings, dark text for contrast
        return 'bg-yellow-500 text-black';
      case 'info':
        // Blue background for informational messages
        return 'bg-blue-600 text-white';
      default:
        // Dark slate fallback for unknown types
        return 'bg-slate-800 text-white';
    }
  }
  

  /* ------------------------------
     Semantic API
  ------------------------------ */
  
  /**
   * Displays a success toast notification (green).
   * Use this after successful operations (e.g., form submission, file upload).
   * 
   * @param message - The success message to display
   * @param duration - Optional duration in milliseconds (default: 5000ms)
   */
  success(message: string, duration?: number): void {
    this.createToast(message, 'success', duration);
  }

  /**
   * Displays an error toast notification (red).
   * Use this when an operation fails or an error occurs.
   * 
   * @param message - The error message to display
   * @param duration - Optional duration in milliseconds (default: 5000ms)
   */
  error(message: string, duration?: number): void {
    this.createToast(message, 'error', duration);
  }

  /**
   * Displays a warning toast notification (yellow).
   * Use this to alert users about something that requires attention but isn't critical.
   * 
   * @param message - The warning message to display
   * @param duration - Optional duration in milliseconds (default: 5000ms)
   */
  warning(message: string, duration?: number): void {
    this.createToast(message, 'warning', duration);
  }

  /**
   * Displays an informational toast notification (blue).
   * Use this for general information or status updates.
   * 
   * @param message - The informational message to display
   * @param duration - Optional duration in milliseconds (default: 5000ms)
   */
  info(message: string, duration?: number): void {
    this.createToast(message, 'info', duration);
  }


}
