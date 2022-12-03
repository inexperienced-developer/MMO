using Firebase;
using Firebase.Auth;
using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Net.Mail;
using UnityEngine;

public class AuthManager : Singleton<AuthManager>
{
    [Header("Firebase")]
    public DependencyStatus DependencyStatus;
    public FirebaseUser User { get; private set; }
    public FirebaseAuth Auth { get; private set; }

    public void SetUser(FirebaseUser user)
    {
        User = user;
    }

    protected override void Awake()
    {
        base.Awake();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus = task.Result;
            if (DependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                IDLogger.LogError($"Could not resolve all Firebase dependencies: {DependencyStatus}");
            }
        });
    }

    private void InitializeFirebase()
    {
        Auth = FirebaseAuth.DefaultInstance;
        Auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (Auth.CurrentUser != User)
        {
            bool signedIn = User != Auth.CurrentUser && Auth.CurrentUser != null;
            if (User != null && !signedIn)
            {
                IDLogger.Log("Signed out " + User.UserId);
            }
            User = Auth.CurrentUser;
            if (signedIn)
            {
                IDLogger.Log("Signed in " + User.UserId);
                //displayName = User.DisplayName ? "";
                //emailAddress = User.Email ?? "";
                //photoUrl = User.PhotoUrl ?? "";
            }
        }
    }

    public void SignIn(string email, string password)
    {
        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                IDLogger.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                IDLogger.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            IDLogger.Log($"User signed in successfully: {newUser.DisplayName} ({newUser.UserId})");
        });
    }

    public void SignUp(string email, string password)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                IDLogger.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                IDLogger.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            FirebaseUser newUser = task.Result;
            IDLogger.Log($"Firebase user created successfully: {newUser.DisplayName} ({newUser.UserId})");
        });
    }
}
