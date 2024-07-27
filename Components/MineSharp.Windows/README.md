## MineSharp.Windows

Provides a Window Api. Create windows and modify them using `WindowClicks`.

### Example:

```csharp
    var window = new Window(0, "Inventory", 3 * 9);
    window.SetSlot(new Slot(diamond_item, 9)); // set window slot 9 to diamond_item
    window.DoSimpleClick(WindowMouseButton.MouseLeft, 9); // pickup slot 9
    window.DoSimpleClick(WindowMouseButton.MouseRight, 10); // put down a single item to slot 10
    window.DoSimpleClick(WindowMouseButton.MouseLeft, 9); // put down the rest of the stack back to slot 9
```