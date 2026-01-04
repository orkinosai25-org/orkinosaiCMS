/**
 * Zoota Chat Agent - JavaScript Interop
 * Provides textarea auto-resize and keyboard handling for the chat interface
 */

window.zootaChatAgent = {
    /**
     * Setup textarea with auto-resize and keyboard shortcuts
     * @param {HTMLElement} textarea - The textarea element reference
     */
    setupTextarea: function(textarea) {
        if (!textarea) {
            console.warn('Zoota Chat Agent: Textarea element not found');
            return;
        }

        // Auto-resize function with debouncing for performance
        let resizeTimeout;
        const autoResize = () => {
            textarea.style.height = 'auto';
            const newHeight = Math.min(textarea.scrollHeight, 120); // Max 120px
            textarea.style.height = newHeight + 'px';
        };

        // Debounced resize handler
        const debouncedResize = () => {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(autoResize, 10);
        };

        // Handle input events for auto-resize
        textarea.addEventListener('input', debouncedResize);

        // Handle keyboard shortcuts
        textarea.addEventListener('keydown', (e) => {
            // Enter to send (without Shift)
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                
                // Find and click the send button
                const sendBtn = textarea.closest('.zoota-input-form')?.querySelector('.zoota-send-btn');
                if (sendBtn && !sendBtn.disabled) {
                    sendBtn.click();
                }
            }
            // Shift+Enter for new line (default behavior, just don't prevent)
        });

        // Focus the textarea when chat panel opens (using requestAnimationFrame for reliability)
        let focusAttempts = 0;
        const maxFocusAttempts = 50; // Maximum 50 attempts (~800ms)
        const focusTextarea = () => {
            if (textarea.offsetParent !== null) {
                textarea.focus();
            } else if (focusAttempts < maxFocusAttempts) {
                focusAttempts++;
                requestAnimationFrame(focusTextarea);
            } else {
                console.warn('Zoota Chat Agent: Could not focus textarea after maximum attempts');
            }
        };
        requestAnimationFrame(focusTextarea);

        // Initial resize
        autoResize();
    },

    /**
     * Scroll chat to bottom (for new messages)
     * @param {string} selector - CSS selector for chat body element
     */
    scrollToBottom: function(selector) {
        const chatBody = document.querySelector(selector || '.zoota-chat-body');
        if (chatBody) {
            chatBody.scrollTop = chatBody.scrollHeight;
        }
    },

    /**
     * Focus the textarea input
     */
    focusInput: function() {
        const textarea = document.querySelector('.zoota-input');
        if (textarea) {
            textarea.focus();
        }
    }
};

// Auto-scroll to bottom when new messages appear
let chatObserver = null;
const observeChatMessages = () => {
    const chatBody = document.querySelector('.zoota-chat-body');
    if (!chatBody) return;

    // Disconnect existing observer if any
    if (chatObserver) {
        chatObserver.disconnect();
    }

    chatObserver = new MutationObserver(() => {
        window.zootaChatAgent.scrollToBottom();
    });

    chatObserver.observe(chatBody, {
        childList: true,
        subtree: true
    });
};

// Cleanup function for when chat is closed
window.zootaChatAgent.cleanup = function() {
    if (chatObserver) {
        chatObserver.disconnect();
        chatObserver = null;
    }
};

// Initialize observers when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', observeChatMessages);
} else {
    observeChatMessages();
}
