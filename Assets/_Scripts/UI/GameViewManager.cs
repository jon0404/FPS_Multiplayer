using UnityEngine;
using System.Collections.Generic;
using PurrNet;

public class GameViewManager : MonoBehaviour
{
    // Keeps a list of all screens in the UI so they can be shown/hidden together.
    [SerializeField] private List<View> allViews = new();
    [SerializeField] private View defaultView;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);

        foreach (var view in allViews)
        {
            if (view != null)
                HideViewInternal(view);
        }

        if (defaultView != null)
            ShowViewInternal(defaultView);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<GameViewManager>();
    }

    public void ShowView<T>(bool hideOthers = true) where T : View
    {
        foreach (var view in allViews)
        {
            if (view == null)
                continue;

            if (view.GetType() == typeof(T))
            {
                ShowViewInternal(view);
            }
            else if (hideOthers)
            {
                HideViewInternal(view);
            }
        }
    }

    public void HideView<T>() where T : View
    {
        foreach (var view in allViews)
        {
            if (view != null && view.GetType() == typeof(T))
                HideViewInternal(view);
        }
    }

    private void ShowViewInternal(View view)
    {
        if (view == null)
            return;

        if (view.canvasGroup != null)
            view.canvasGroup.alpha = 1;

        view.OnShow();
    }

    private void HideViewInternal(View view)
    {
        if (view == null)
            return;

        if (view.canvasGroup != null)
            view.canvasGroup.alpha = 0;

        view.OnHide();
    }
}

public abstract class View : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public abstract void OnShow();
    public abstract void OnHide();
}