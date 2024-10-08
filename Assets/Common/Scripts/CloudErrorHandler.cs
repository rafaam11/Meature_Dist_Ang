﻿/*===============================================================================
Copyright (c) 2018 PTC Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/
using UnityEngine;
using Vuforia;

public class CloudErrorHandler : MonoBehaviour
{
    bool mIsRestartNeeded;
    string mErrorTitle;
    string mErrorMessage;
    CloudRecoBehaviour mCloudRecoBehaviour;
    
    void Start()
    {
        // Register this event handler with CloudRecoBehaviour
        mCloudRecoBehaviour = FindObjectOfType<CloudRecoBehaviour>();

        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
            mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
        }

        if (VuforiaConfiguration.Instance.Vuforia.LicenseKey == string.Empty && mCloudRecoBehaviour)
        {
            mErrorTitle = "Cloud Reco Init Error";
            mErrorMessage = "Vuforia License Key not found. Cloud Reco requires a valid license.";

            MessageBox.DisplayMessageBox(mErrorTitle, mErrorMessage, false, null);
        }
    }

    void OnDestroy()
    {
        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.UnregisterOnInitErrorEventHandler(OnInitError);
            mCloudRecoBehaviour.UnregisterOnUpdateErrorEventHandler(OnUpdateError);
        }
    }

    /// <summary>
    /// Called if Cloud Reco initialization fails
    /// </summary>
    public void OnInitError(ObjectRecoBehaviour.InitError initError)
    {
        switch (initError)
        {
            case ObjectRecoBehaviour.InitError.NO_NETWORK_CONNECTION:
                mIsRestartNeeded = true;
                mErrorTitle = "Network Unavailable";
                mErrorMessage = "Please check your Internet connection and try again.";
                break;
            case ObjectRecoBehaviour.InitError.SERVICE_NOT_AVAILABLE:
                mErrorTitle = "Service Unavailable";
                mErrorMessage = "Failed to initialize app because the service is not available.";
                break;
        }

        // Prepend the error code in red
        mErrorMessage = "<color=red>" + initError.ToString().Replace("_", " ") + "</color>\n\n" + mErrorMessage;

        // Remove rich text tags for console logging
        var errorTextConsole = mErrorMessage.Replace("<color=red>", "").Replace("</color>", "");

        Debug.LogError("OnInitError() - Initialization Error: " + initError + "\n\n" + errorTextConsole);

        MessageBox.DisplayMessageBox(mErrorTitle, mErrorMessage, true, CloseDialog);
    }

    /// <summary>
    /// Called if a Cloud Reco update error occurs
    /// </summary>
    public void OnUpdateError(ObjectRecoBehaviour.QueryError updateError)
    {
        switch (updateError)
        {
            case ObjectRecoBehaviour.QueryError.AUTHORIZATION_FAILED:
                mErrorTitle = "Authorization Error";
                mErrorMessage = "The cloud recognition service access keys are incorrect or have expired.";
                break;
            case ObjectRecoBehaviour.QueryError.NO_NETWORK_CONNECTION:
                mErrorTitle = "Network Unavailable";
                mErrorMessage = "Please check your Internet connection and try again.";
                break;
            case ObjectRecoBehaviour.QueryError.PROJECT_SUSPENDED:
                mErrorTitle = "Authorization Error";
                mErrorMessage = "The cloud recognition service has been suspended.";
                break;
            case ObjectRecoBehaviour.QueryError.REQUEST_TIMEOUT:
                mErrorTitle = "Request Timeout";
                mErrorMessage = "The network request has timed out, please check your Internet connection and try again.";
                break;
            case ObjectRecoBehaviour.QueryError.SERVICE_NOT_AVAILABLE:
                mErrorTitle = "Service Unavailable";
                mErrorMessage = "The service is unavailable, please try again later.";
                break;
            case ObjectRecoBehaviour.QueryError.TIMESTAMP_OUT_OF_RANGE:
                mErrorTitle = "Clock Sync Error";
                mErrorMessage = "Please update the date and time and try again.";
                break;
            case ObjectRecoBehaviour.QueryError.UPDATE_SDK:
                mErrorTitle = "Unsupported Version";
                mErrorMessage = "The application is using an unsupported version of Vuforia.";
                break;
            case ObjectRecoBehaviour.QueryError.BAD_FRAME_QUALITY:
                mErrorTitle = "Bad Frame Quality";
                mErrorMessage = "Low-frame quality has been continuously observed.\n\nError Event Received on Frame: " + Time.frameCount;
                break;
        }

        // Prepend the error code in red
        mErrorMessage = "<color=red>" + updateError.ToString().Replace("_", " ") + "</color>\n\n" + mErrorMessage;

        // Remove rich text tags for console logging
        var errorTextConsole = mErrorMessage.Replace("<color=red>", "").Replace("</color>", "");

        Debug.LogError("OnUpdateError() - Update Error: " + updateError + "\n\n" + errorTextConsole);

        MessageBox.DisplayMessageBox(mErrorTitle, mErrorMessage, true, CloseDialog);
    }

    public void CloseDialog()
    {
        if (mIsRestartNeeded) 
            RestartApplication();
    }

    // Callback for network-not-available error message
    void RestartApplication()
    {
        int startLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
        if (startLevel < 0) 
            startLevel = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(startLevel);
    }
}
