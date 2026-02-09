# Bootstrap 5.3.3 & Bootstrap Icons 1.11.3 - Usage Guide

## ? Updated to Latest Versions

### Bootstrap 5.3.3
- **CSS CDN**: https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css
- **JS CDN**: https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js
- **Includes**: Popper.js for tooltips, popovers, and dropdowns
- **SRI Hash**: Added for security

### Bootstrap Icons 1.11.3
- **CDN**: https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css
- **Icon Count**: 2,000+ icons
- **SRI Hash**: Added for security

## ?? Bootstrap Icons Already in Use

### Navigation
- `bi-lightning-charge-fill` - Brand logo
- `bi-shield-shaded` - Polly Demo
- `bi-cloud-sun` - Open Weather
- `bi-collection` - Bulk Calls
- `bi-braces` - API Documentation
- `bi-palette` - Theme Switcher

### Footer
- `bi-code-square` - Version info
- `bi-github` - GitHub link
- `bi-c-circle` - Copyright symbol
- `bi-clock-history` - Build date
- `bi-person-badge` - Author link

### Content Pages
- `bi-file-earmark-text` - Documentation
- `bi-arrow-right-circle` - Call-to-action buttons
- `bi-code-slash` - API documentation section

## ?? Recommended Bootstrap Icons to Add

### Common UI Elements
```html
<!-- Loading States -->
<i class="bi bi-hourglass-split"></i> <!-- Loading -->
<i class="bi bi-arrow-clockwise spinner"></i> <!-- Refreshing -->

<!-- Success/Error/Warning -->
<i class="bi bi-check-circle-fill text-success"></i> <!-- Success -->
<i class="bi bi-x-circle-fill text-danger"></i> <!-- Error -->
<i class="bi bi-exclamation-triangle-fill text-warning"></i> <!-- Warning -->
<i class="bi bi-info-circle-fill text-info"></i> <!-- Info -->

<!-- Navigation & Actions -->
<i class="bi bi-arrow-left-circle"></i> <!-- Back -->
<i class="bi bi-arrow-right-circle"></i> <!-- Forward/Next -->
<i class="bi bi-download"></i> <!-- Download -->
<i class="bi bi-upload"></i> <!-- Upload -->
<i class="bi bi-trash"></i> <!-- Delete -->
<i class="bi bi-pencil"></i> <!-- Edit -->
<i class="bi bi-plus-circle"></i> <!-- Add/Create -->
<i class="bi bi-search"></i> <!-- Search -->
<i class="bi bi-filter"></i> <!-- Filter -->

<!-- Data & Analytics -->
<i class="bi bi-graph-up"></i> <!-- Statistics/Growth -->
<i class="bi bi-bar-chart"></i> <!-- Analytics -->
<i class="bi bi-pie-chart"></i> <!-- Reports -->
<i class="bi bi-clipboard-data"></i> <!-- Data -->

<!-- Communication -->
<i class="bi bi-envelope"></i> <!-- Email -->
<i class="bi bi-bell"></i> <!-- Notifications -->
<i class="bi bi-chat-dots"></i> <!-- Messages -->

<!-- Settings & Configuration -->
<i class="bi bi-gear"></i> <!-- Settings -->
<i class="bi bi-sliders"></i> <!-- Advanced settings -->
<i class="bi bi-toggle-on"></i> <!-- Enable/Disable -->

<!-- User & Security -->
<i class="bi bi-person"></i> <!-- User -->
<i class="bi bi-shield-check"></i> <!-- Security -->
<i class="bi bi-lock"></i> <!-- Locked -->
<i class="bi bi-unlock"></i> <!-- Unlocked -->

<!-- Files & Documents -->
<i class="bi bi-file-earmark"></i> <!-- File -->
<i class="bi bi-folder"></i> <!-- Folder -->
<i class="bi bi-file-code"></i> <!-- Code file -->
<i class="bi bi-file-text"></i> <!-- Text file -->
```

## ?? Bootstrap 5.3.3 New Features & Best Practices

### Color Mode Support (Light/Dark)
```html
<html lang="en" data-bs-theme="dark">
<!-- Automatically switches between light and dark themes -->
```

### Enhanced Grid System
```html
<!-- Use gap utilities instead of gutter classes -->
<div class="row g-3"> <!-- gap of 1rem -->
<div class="row g-4"> <!-- gap of 1.5rem -->
<div class="row g-5"> <!-- gap of 3rem -->
```

### Container Breakpoints
```html
<div class="container-sm">  <!-- 540px -->
<div class="container-md">  <!-- 720px -->
<div class="container-lg">  <!-- 960px -->
<div class="container-xl">  <!-- 1140px -->
<div class="container-xxl"> <!-- 1320px -->
```

### Utility Classes
```html
<!-- Text Utilities -->
<p class="text-truncate">...</p>
<p class="text-break">...</p>
<p class="text-wrap">...</p>
<p class="text-nowrap">...</p>

<!-- Background Utilities -->
<div class="bg-primary bg-opacity-75">...</div>
<div class="bg-gradient">...</div>

<!-- Border Utilities -->
<div class="border border-primary border-2 rounded-3">...</div>

<!-- Shadow Utilities -->
<div class="shadow-sm">...</div>
<div class="shadow">...</div>
<div class="shadow-lg">...</div>

<!-- Display Utilities -->
<div class="d-flex justify-content-between align-items-center">...</div>
<div class="d-grid gap-2">...</div>

<!-- Position Utilities -->
<div class="position-relative">
    <div class="position-absolute top-0 start-0">...</div>
</div>

<!-- Sizing Utilities -->
<div class="w-25 h-25">...</div> <!-- 25% width & height -->
<div class="w-50 h-50">...</div> <!-- 50% width & height -->
<div class="w-75 h-75">...</div> <!-- 75% width & height -->
<div class="w-100 h-100">...</div> <!-- 100% width & height -->

<!-- Spacing Utilities -->
<div class="m-3 p-3">...</div> <!-- margin & padding of 1rem -->
<div class="mx-auto">...</div> <!-- center horizontally -->
<div class="my-3">...</div> <!-- margin top & bottom -->
```

### Form Validation
```html
<input type="text" class="form-control is-valid" required>
<div class="valid-feedback">Looks good!</div>

<input type="text" class="form-control is-invalid" required>
<div class="invalid-feedback">Please provide a value.</div>
```

### Offcanvas Component
```html
<button class="btn btn-primary" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasRight">
    Toggle Offcanvas
</button>

<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasRight">
    <div class="offcanvas-header">
        <h5 class="offcanvas-title">Offcanvas</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas"></button>
    </div>
    <div class="offcanvas-body">
        Content goes here
    </div>
</div>
```

### Toast Notifications
```html
<div class="toast align-items-center" role="alert">
    <div class="d-flex">
        <div class="toast-body">
            Hello, world! This is a toast message.
        </div>
        <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast"></button>
    </div>
</div>
```

## ?? Recommended Enhancements for AsyncSpark

### 1. Add Loading States with Bootstrap Icons
```razor
<button class="btn btn-primary" disabled>
    <i class="bi bi-hourglass-split spinner-border spinner-border-sm me-2"></i>
    Loading...
</button>
```

### 2. Enhanced Alert Messages
```razor
<div class="alert alert-success d-flex align-items-center" role="alert">
    <i class="bi bi-check-circle-fill me-2"></i>
    <div>Operation completed successfully!</div>
</div>
```

### 3. Better Form Validation Feedback
```razor
<div class="mb-3">
    <label class="form-label">
        <i class="bi bi-envelope me-1"></i>Email
    </label>
    <input type="email" class="form-control" required>
    <div class="invalid-feedback">
        <i class="bi bi-exclamation-circle me-1"></i>Please provide a valid email
    </div>
</div>
```

### 4. Enhanced Cards
```razor
<div class="card shadow-sm">
    <div class="card-header bg-primary text-white">
        <i class="bi bi-graph-up me-2"></i>Statistics
    </div>
    <div class="card-body">
        <h5 class="card-title">Performance Metrics</h5>
        <p class="card-text">View your application's performance data.</p>
    </div>
</div>
```

### 5. Breadcrumbs with Icons
```razor
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item">
            <a href="/"><i class="bi bi-house-door me-1"></i>Home</a>
        </li>
        <li class="breadcrumb-item active">
            <i class="bi bi-cloud-sun me-1"></i>Weather
        </li>
    </ol>
</nav>
```

## ?? Resources

- **Bootstrap 5.3 Docs**: https://getbootstrap.com/docs/5.3/getting-started/introduction/
- **Bootstrap Icons**: https://icons.getbootstrap.com/
- **Bootstrap Examples**: https://getbootstrap.com/docs/5.3/examples/
- **Cheat Sheet**: https://getbootstrap.com/docs/5.3/examples/cheatsheet/

## ?? Security Best Practices

1. ? **SRI Hashes**: Always include integrity hashes for CDN resources
2. ? **HTTPS**: Use HTTPS CDN links
3. ? **crossorigin**: Include crossorigin="anonymous" attribute
4. ? **Minified Files**: Use .min.css and .min.js for production

## ?? Current Theme System

Your application already has an excellent theme switcher with Bootswatch integration. The latest Bootstrap version is fully compatible with all Bootswatch themes.

## ?? Local vs CDN

**Current Setup**: Using CDN (? Recommended for most cases)

**Benefits**:
- Faster load times (browser caching)
- Reduced server bandwidth
- Automatic updates (when you change version)
- Reliability (CDN uptime)

**When to use local**:
- Offline applications
- Custom Bootstrap builds
- Corporate networks blocking CDNs
