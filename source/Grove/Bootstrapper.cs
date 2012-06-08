﻿namespace Grove
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Windows;
  using Caliburn.Micro;
  using Castle.MicroKernel.Lifestyle.Scoped;
  using Ui.Shell;
  using Ui.StartScreen;

  public class Bootstrapper : Bootstrapper<IShell>
  {
    public static IoC Container;

    protected override void Configure()
    {
      AppDomain.CurrentDomain.UnhandledException += (s, a) => MessageBox.Show(a.ExceptionObject.ToString(), "BUUUUU :(");

      Container = IoC.Ui();
      ConfigureCaliburn();
    }
    
    protected override void DisplayRootView()
    {
      var shell = Container.Resolve<Shell>();
      var start = Container.Resolve<ViewModel>();

      shell.ChangeScreen(start);
      new WindowManager().ShowWindow(shell);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
      return Container.ResolveAll(service).Cast<object>();
    }

    protected override object GetInstance(Type service, string key)
    {
      return String.IsNullOrEmpty(key)
        ? Container.Resolve(service)
        : Container.Resolve(key, service);
    }

    private static void ConfigureCaliburn()
    {
      ConfigureViewLocator();
    }

    private static void ConfigureViewLocator()
    {
      ViewLocator.LocateForModelType = (presenter, displayLocation, context) => {
        if (presenter.Name.Contains("Proxy"))
        {
          presenter = presenter.BaseType;
        }

        var viewType = context == null
          ? Assembly.GetExecutingAssembly().GetType(presenter.Namespace + ".View")
          : Assembly.GetExecutingAssembly().GetType(presenter.Namespace + "." + context.ToString());

        if (viewType == null)
          throw new InvalidOperationException(
            String.Format("Could not find View for ViewModel: {0}.", presenter));

        return ViewLocator.GetOrCreateViewType(viewType);
      };
    }

    private static ILifetimeScope _scope = null;

    public static ILifetimeScope GetScope()
    {
      return _scope ?? (_scope = new DefaultLifetimeScope());
    }

    public static void NewGame()
    {
      if (_scope != null)
        _scope.Dispose();

      _scope = null;
    }
  }
}