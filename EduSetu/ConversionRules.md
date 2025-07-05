# React to Blazor Conversion Rules

## Overview
This document outlines the rules and patterns for converting React components to Blazor Server components, ensuring consistency and maintainability across the EduSetu project.

## File Structure

### React → Blazor File Mapping
```
React: src/components/ComponentName.tsx
Blazor: Components/Pages/ComponentName.razor (for pages)
Blazor: Components/Shared/ComponentName.razor (for shared components)
Blazor: Components/Layout/ComponentName.razor (for layout components)
```

### Naming Conventions
- **React**: PascalCase for components (e.g., `Header.tsx`)
- **Blazor**: PascalCase for components (e.g., `Header.razor`)
- **Routes**: React uses `/component` → Blazor uses `@page "/component"`

## Component Structure

### React Component Structure
```tsx
import React, { useState, useEffect } from "react";

interface Props {
  title: string;
  onClick: () => void;
}

const ComponentName: React.FC<Props> = ({ title, onClick }) => {
  const [state, setState] = useState("");
  
  useEffect(() => {
    // side effects
  }, []);

  return (
    <div className="container">
      <h1>{title}</h1>
      <button onClick={onClick}>Click me</button>
    </div>
  );
};

export default ComponentName;
```

### Blazor Component Structure
```razor
@page "/component"
@using System.ComponentModel.DataAnnotations
@inject NavigationManager NavigationManager

<div class="container">
    <h1>@title</h1>
    <button @onclick="HandleClick">Click me</button>
</div>

@code {
    [Parameter]
    public string Title { get; set; } = "";

    [Parameter]
    public EventCallback OnClick { get; set; }

    private string state = "";

    protected override void OnInitialized()
    {
        // initialization
    }

    private async Task HandleClick()
    {
        await OnClick.InvokeAsync();
    }
}
```

## State Management

### React State → Blazor State
```tsx
// React
const [count, setCount] = useState(0);
const [user, setUser] = useState({ name: "", email: "" });
```

```csharp
// Blazor
private int count = 0;
private User user = new() { Name = "", Email = "" };

// For complex state updates
private void UpdateUser(string name, string email)
{
    user = user with { Name = name, Email = email };
    StateHasChanged();
}
```

## Event Handling

### React Events → Blazor Events
```tsx
// React
<button onClick={handleClick}>Click</button>
<input onChange={(e) => setValue(e.target.value)} />
<form onSubmit={handleSubmit}>
```

```razor
// Blazor
<button @onclick="HandleClick">Click</button>
<InputText @bind-value="value" />
<EditForm Model="formData" OnValidSubmit="HandleSubmit">
```

## Form Handling

### React Forms → Blazor Forms
```tsx
// React
const [formData, setFormData] = useState({
  email: "",
  password: ""
});

const handleSubmit = (e) => {
  e.preventDefault();
  // submit logic
};

<form onSubmit={handleSubmit}>
  <input 
    type="email" 
    value={formData.email}
    onChange={(e) => setFormData({...formData, email: e.target.value})}
  />
</form>
```

```razor
// Blazor
@code {
    private LoginFormData formData = new();

    public class LoginFormData
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";
    }

    private async Task HandleSubmit()
    {
        // submit logic
    }
}

<EditForm Model="formData" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <InputText @bind-value="formData.Email" type="email" />
    <ValidationMessage For="@(() => formData.Email)" />
</EditForm>
```

## Styling Conversion

### CSS Classes
```tsx
// React
<div className="container mx-auto px-4 bg-white shadow-lg">
  <button className="bg-blue-500 hover:bg-blue-700 text-white px-4 py-2 rounded">
```

```razor
// Blazor
<div class="container mx-auto px-4 bg-white shadow-lg">
  <button class="bg-blue-500 hover:bg-blue-700 text-white px-4 py-2 rounded">
```

### Conditional Styling
```tsx
// React
<div className={`container ${isActive ? "bg-blue-500" : "bg-gray-300"}`}>
```

```razor
// Blazor
<div class="@($"container {(isActive ? "bg-blue-500" : "bg-gray-300")}")">
```

## Navigation

### React Router → Blazor Navigation
```tsx
// React
import { useNavigate } from "react-router-dom";

const navigate = useNavigate();
navigate("/login");
```

```razor
// Blazor
@inject NavigationManager NavigationManager

NavigationManager.NavigateTo("/login");
```

### Links
```tsx
// React
<Link to="/login">Login</Link>
```

```razor
// Blazor
<NavLink href="/login">Login</NavLink>
```

## Lifecycle Methods

### React Lifecycle → Blazor Lifecycle
```tsx
// React
useEffect(() => {
  // componentDidMount
  return () => {
    // componentWillUnmount
  };
}, []);

useEffect(() => {
  // componentDidUpdate
}, [dependency]);
```

```csharp
// Blazor
protected override void OnInitialized()
{
    // componentDidMount equivalent
}

protected override void OnAfterRender(bool firstRender)
{
    if (firstRender)
    {
        // first render logic
    }
}

public async ValueTask DisposeAsync()
{
    // cleanup logic
}
```

## JavaScript Interop

### React useEffect → Blazor JS Interop
```tsx
// React
useEffect(() => {
  const handleScroll = () => {
    // scroll logic
  };
  window.addEventListener("scroll", handleScroll);
  return () => window.removeEventListener("scroll", handleScroll);
}, []);
```

```csharp
// Blazor
@inject IJSRuntime JSRuntime
@implements IAsyncDisposable

private DotNetObjectReference<ComponentName>? objRef;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        objRef = DotNetObjectReference.Create(this);
        await JSRuntime.InvokeVoidAsync("addScrollListener", objRef);
    }
}

[JSInvokable]
public void OnScroll(bool scrolled)
{
    // handle scroll
    StateHasChanged();
}

public async ValueTask DisposeAsync()
{
    if (objRef is not null)
    {
        objRef.Dispose();
    }
}
```

## Data Binding

### React Binding → Blazor Binding
```tsx
// React
<input 
  value={value} 
  onChange={(e) => setValue(e.target.value)} 
/>
```

```razor
// Blazor
<InputText @bind-value="value" />
```

### Two-way Binding
```tsx
// React
<input 
  value={formData.email} 
  onChange={(e) => setFormData({...formData, email: e.target.value})} 
/>
```

```razor
// Blazor
<InputText @bind-value="formData.Email" />
```

## Conditional Rendering

### React Conditional → Blazor Conditional
```tsx
// React
{isLoading && <Spinner />}
{user ? <UserProfile user={user} /> : <LoginForm />}
```

```razor
// Blazor
@if (isLoading)
{
    <Spinner />
}

@if (user != null)
{
    <UserProfile User="user" />
}
else
{
    <LoginForm />
}
```

## Lists and Loops

### React Map → Blazor Foreach
```tsx
// React
{items.map(item => (
  <div key={item.id}>{item.name}</div>
))}
```

```razor
// Blazor
@foreach (var item in items)
{
    <div>@item.Name</div>
}
```

## Props and Parameters

### React Props → Blazor Parameters
```tsx
// React
interface Props {
  title: string;
  onClick: () => void;
  children?: React.ReactNode;
}

const Component: React.FC<Props> = ({ title, onClick, children }) => {
```

```csharp
// Blazor
[Parameter]
public string Title { get; set; } = "";

[Parameter]
public EventCallback OnClick { get; set; }

[Parameter]
public RenderFragment? ChildContent { get; set; }
```

## Validation

### React Validation → Blazor Validation
```tsx
// React
const [errors, setErrors] = useState({});

const validate = () => {
  const newErrors = {};
  if (!email) newErrors.email = "Email is required";
  setErrors(newErrors);
};
```

```razor
// Blazor
public class FormData
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = "";
}

<EditForm Model="formData" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <InputText @bind-value="formData.Email" />
    <ValidationMessage For="@(() => formData.Email)" />
</EditForm>
```

## Common Patterns

### Loading States
```tsx
// React
const [isLoading, setIsLoading] = useState(false);

<button disabled={isLoading}>
  {isLoading ? <Spinner /> : "Submit"}
</button>
```

```razor
// Blazor
private bool isLoading = false;

<button disabled="@isLoading">
    @if (isLoading)
    {
        <div class="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
    }
    else
    {
        <span>Submit</span>
    }
</button>
```

### Error Handling
```tsx
// React
const [error, setError] = useState(null);

{error && <div className="error">{error}</div>}
```

```razor
// Blazor
private string? error = null;

@if (!string.IsNullOrEmpty(error))
{
    <div class="error">@error</div>
}
```

## Best Practices

### 1. Component Organization
- Keep components focused and single-purpose
- Use proper separation of concerns
- Group related functionality together

### 2. State Management
- Use private fields for local state
- Use parameters for props
- Use EventCallback for callbacks

### 3. Performance
- Use `StateHasChanged()` sparingly
- Implement `IAsyncDisposable` for cleanup
- Use `OnAfterRenderAsync` for JS interop

### 4. Validation
- Use DataAnnotations for form validation
- Provide clear error messages
- Use ValidationMessage components

### 5. Styling
- Use Tailwind CSS classes consistently
- Follow responsive design patterns
- Maintain accessibility standards

### 6. Navigation
- Use NavLink for internal navigation
- Use NavigationManager for programmatic navigation
- Handle route parameters properly

## Common Conversion Checklist

- [ ] Convert file extension from `.tsx` to `.razor`
- [ ] Add `@page` directive for pages
- [ ] Add necessary `@using` statements
- [ ] Add `@inject` for services (NavigationManager, IJSRuntime, etc.)
- [ ] Convert React hooks to Blazor lifecycle methods
- [ ] Convert `useState` to private fields
- [ ] Convert `useEffect` to `OnInitialized`/`OnAfterRenderAsync`
- [ ] Convert event handlers to Blazor event syntax
- [ ] Convert form handling to `EditForm` with validation
- [ ] Convert conditional rendering syntax
- [ ] Convert list rendering to `@foreach`
- [ ] Convert props to `[Parameter]` attributes
- [ ] Convert CSS classes (className → class)
- [ ] Convert navigation (useNavigate → NavigationManager)
- [ ] Convert JS interop patterns
- [ ] Test all functionality
- [ ] Ensure responsive design works
- [ ] Verify accessibility features

## Troubleshooting

### Common Issues
1. **Binding not working**: Use `@bind-value` instead of `@bind`
2. **Events not firing**: Use `@onclick` instead of `onClick`
3. **Validation not showing**: Add `DataAnnotationsValidator` and `ValidationMessage`
4. **Navigation not working**: Use `NavigationManager.NavigateTo()`
5. **JS interop errors**: Implement `IAsyncDisposable` and dispose `DotNetObjectReference`

### Performance Tips
1. Use `StateHasChanged()` only when necessary
2. Implement proper cleanup in `DisposeAsync`
3. Use `OnAfterRenderAsync` for JS interop
4. Avoid unnecessary re-renders

This conversion guide ensures consistent, maintainable, and performant Blazor components that match the functionality and design of the original React components. 