# Zoota AI Assistant - Visual Guide

## Chat Button (Bottom-Right Corner)

The Zoota chat button appears as a circular button in the bottom-right corner of the admin panel:

- **Color**: Blue gradient (Azure brand colors)
- **Size**: 75px × 75px
- **Animation**: Pulsing effect
- **Icon**: Zoota logo (friendly robot/AI character)
- **Position**: Fixed, always visible while scrolling

### Button States

1. **Default**: Blue gradient with pulse animation
2. **Hover**: Scales up slightly (1.1x) with enhanced shadow
3. **Active**: Scales down (0.95x) for click feedback

## Chat Panel (Opened)

When the button is clicked, a chat panel slides up:

- **Size**: 380px × 600px (desktop), responsive on mobile
- **Position**: Bottom-right, anchored to screen
- **Background**: White with subtle shadow
- **Radius**: 16px rounded corners
- **Animation**: Smooth slide-up transition

### Chat Panel Sections

#### 1. Header (Blue Gradient)
- **Zoota Avatar**: Professional AI icon
- **Title**: "Zoota AI Assistant"
- **Status Indicator**: Green dot + "Online - Powered by Azure AI"
- **Close Button**: × icon in top-right

#### 2. Body (Light Gray Background)

**Welcome Screen** (no messages):
- Large Zoota logo
- Greeting: "Hi! I'm Zoota 👋"
- Description of capabilities
- Three quick action buttons:
  - "Tell me about OrkinosAI"
  - "What services do you offer?"
  - "How can AI help my business?"

**Chat Messages** (with conversation):
- **User messages**: Blue gradient bubbles, aligned right
- **Assistant messages**: White bubbles with avatar, aligned left
- **System messages**: Light blue pill-shaped, centered
- **Typing indicator**: Three animated dots
- **Timestamps**: Small gray text below messages

#### 3. Footer (White Background)

- **Input Area**: 
  - Auto-resizing textarea (min 44px, max 120px)
  - Placeholder: "Type your message... (Press Enter to send...)"
  - Rounded corners (24px)
  - Blue border on focus
  
- **Send Button**:
  - Circular button (44px)
  - Blue gradient background
  - Paper plane icon
  - Disabled when textarea empty or typing

- **Disclaimer**: 
  - "Zoota is an AI assistant. Responses may not always be accurate."
  - Small gray text

## Color Scheme (Azure/Fluent Design)

### Primary Colors
- **Azure Blue**: `#0078D4` (primary brand)
- **Light Blue**: `#00BCF2` (gradient end)
- **Accent Blue**: `#50E6FF` (highlights, status dots)

### Neutral Colors
- **Background**: `#F5F5F5` (light gray)
- **Text**: `#333333` (dark gray)
- **Secondary Text**: `#666666` (medium gray)
- **Disabled**: `#999999` (light gray)

### States
- **Hover**: Slightly darker shade with enhanced shadow
- **Active**: Scale transform with pressed effect
- **Focus**: Blue outline

## Typography

- **Font Family**: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif
- **Header Title**: 18px, 600 weight
- **Body Text**: 14px, normal weight
- **Status Text**: 12px, 0.9 opacity
- **Disclaimer**: 11px, gray color

## Responsive Behavior

### Mobile (< 480px)
- Chat panel expands to full width minus margins
- Button slightly smaller (65px × 65px)
- Touch-optimized tap targets

### Tablet (480px - 768px)
- Standard desktop layout
- Optimized spacing

### Desktop (> 768px)
- Fixed 380px width
- Maximum height calc(100vh - 100px)
- Scrollable content area

## Animations

### Button Pulse
```css
@keyframes pulse {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.2); opacity: 0.5; }
}
```

### Panel Slide-Up
```css
@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}
```

### Typing Indicator
```css
@keyframes typing {
  0%, 60%, 100% { transform: translateY(0); opacity: 0.7; }
  30% { transform: translateY(-10px); opacity: 1; }
}
```

### Message Fade-In
```css
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
```

## Accessibility

- **Keyboard Navigation**: Full support with Tab key
- **Screen Readers**: ARIA labels on interactive elements
- **Focus Indicators**: Visible blue outline on focus
- **Touch Targets**: Minimum 44px for mobile usability
- **Color Contrast**: WCAG AA compliant

## Browser Compatibility

- ✅ Chrome/Edge (Chromium) - Full support
- ✅ Firefox - Full support
- ✅ Safari - Full support
- ✅ Mobile browsers (iOS/Android) - Full support
- ⚠️ IE11 - Not supported (use Edge)

## Performance Optimizations

- **CSS Containment**: Isolated layout/paint for chat panel
- **Lazy Rendering**: Chat only renders when opened
- **Debounced Auto-Resize**: Textarea resize throttled
- **Efficient Animations**: GPU-accelerated transforms
- **Minimal Reflows**: Fixed positioning

## Example Screenshots

### Closed State
```
[Admin Panel]
...
[Content Area]
...
                                    [🤖] ← Zoota Button
```

### Opened State - Welcome
```
┌─────────────────────────────────────┐
│ 🤖 Zoota AI Assistant         [ × ] │
│ ⚫ Online - Powered by Azure AI     │
├─────────────────────────────────────┤
│                                     │
│           [Zoota Logo]              │
│                                     │
│      Hi! I'm Zoota 👋               │
│                                     │
│  Your friendly AI assistant...      │
│                                     │
│  ┌─────────────────────────────┐   │
│  │ Tell me about OrkinosAI     │   │
│  └─────────────────────────────┘   │
│  ┌─────────────────────────────┐   │
│  │ What services do you offer? │   │
│  └─────────────────────────────┘   │
│  ┌─────────────────────────────┐   │
│  │ How can AI help my business?│   │
│  └─────────────────────────────┘   │
│                                     │
├─────────────────────────────────────┤
│ [Type your message...        ] [>] │
│ Zoota is an AI assistant...        │
└─────────────────────────────────────┘
```

### Opened State - Conversation
```
┌─────────────────────────────────────┐
│ 🤖 Zoota AI Assistant         [ × ] │
│ ⚫ Online - Powered by Azure AI     │
├─────────────────────────────────────┤
│                                     │
│ 🤖 How can I help you?              │
│    14:23                            │
│                                     │
│              Hello Zoota! 💬        │
│                           14:24    │
│                                     │
│ 🤖 Hi! I'm here to help with       │
│    your CMS. What would you         │
│    like to do?                      │
│    14:24                            │
│                                     │
│              Create a new page 💬   │
│                           14:25    │
│                                     │
│ 🤖 ⚫⚫⚫ typing...                   │
│                                     │
├─────────────────────────────────────┤
│ [Type your message...        ] [>] │
│ Zoota is an AI assistant...        │
└─────────────────────────────────────┘
```

---

For more details, see the [Zoota User Guide](ZOOTA_USER_GUIDE.md).
