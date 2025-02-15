using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using UIButton = UnityEngine.UI.Button; // Alias for clarity

namespace MikeNspired.VRConsoleLog
{
    public class LogPool
    {
        private VRConsoleLogger vrConsoleLogger;
        internal Queue<LogMessage> _logMessagePool = new();
        private bool _poolInitialized = false;
        public int _activeMessageCount = 0;

        public LogPool(VRConsoleLogger vrConsoleLogger) => this.vrConsoleLogger = vrConsoleLogger;

        internal void InitializeLogMessagePool(Action rebuildUIAction)
        {
            if (_poolInitialized)
                return;

            UnityAction unityAction = new UnityAction(rebuildUIAction);

            // First, add any existing log messages already in the parent to the pool.
            foreach (Transform child in vrConsoleLogger.LogMessageParent)
            {
                LogMessage existingMessage = child.GetComponent<LogMessage>();
                if (existingMessage != null)
                {
                    existingMessage.gameObject.SetActive(false);
                    existingMessage.Button.onClick.AddListener(unityAction); // Subscribe here
                    _logMessagePool.Enqueue(existingMessage);
                }
            }

            // Calculate how many new messages need to be instantiated.
            int additionalNeeded = vrConsoleLogger.MaxMessageCount - _logMessagePool.Count;

            for (int i = 0; i < additionalNeeded; i++)
            {
                LogMessage messageInstance = Object.Instantiate(vrConsoleLogger.LogMessagePrefab, vrConsoleLogger.LogMessageParent).GetComponent<LogMessage>();
                messageInstance.gameObject.SetActive(false);
                messageInstance.Button.onClick.AddListener(unityAction); // Subscribe here
                _logMessagePool.Enqueue(messageInstance);
            }
            _poolInitialized = true;
        }

        internal LogMessage GetLogMessageFromPool()
        {
            LogMessage message;
            if (_logMessagePool.Count == 0)
            {
                // Recycle the oldest active message if the pool is empty
                message = vrConsoleLogger.LogMessageParent.GetChild(0).GetComponent<LogMessage>();
                message.gameObject.SetActive(false); // Deactivate it first
                _logMessagePool.Enqueue(message); // Then enqueue it
            }

            // Dequeue the next available message from the pool
            message = _logMessagePool.Dequeue();
            message.gameObject.SetActive(true);
            message.isActive = true;
            message.transform.SetAsLastSibling();
            _activeMessageCount++; // Increment active count as this message is now active

            return message;
        }

        internal void ReturnLogMessageToPool(LogMessage message)
        {
            message.gameObject.SetActive(false);
            message.isActive = false;
            _logMessagePool.Enqueue(message);
            _activeMessageCount--;
        }

        internal void AdjustPoolSize()
        {
            int currentActiveAndPooled = _logMessagePool.Count + _activeMessageCount;
        
            // If we have more messages than needed
            if (currentActiveAndPooled > vrConsoleLogger.MaxMessageCount)
            {
                int excess = currentActiveAndPooled - vrConsoleLogger.MaxMessageCount;
                RemoveExcessMessages(excess);
            }
            else if (currentActiveAndPooled < vrConsoleLogger.MaxMessageCount)
            {
                // Add messages if we have fewer than needed
                AddMessagesToPool(vrConsoleLogger.MaxMessageCount - currentActiveAndPooled);
            }
        }

        private void RemoveExcessMessages(int excess)
        {
            // Start by removing inactive messages from the pool
            while (excess > 0 && _logMessagePool.Count > 0)
            {
                var messageToRemove = _logMessagePool.Dequeue();
                Object.Destroy(messageToRemove.gameObject);
                excess--;
            }

            // Remove active messages if still necessary
            while (excess > 0 && _activeMessageCount > 0)
            {
                LogMessage messageToRemove = FindLastActiveMessage();
                if (messageToRemove != null)
                {
                    messageToRemove.gameObject.SetActive(false);
                    messageToRemove.Button.onClick.RemoveAllListeners();
                    Object.Destroy(messageToRemove.gameObject);
                    _activeMessageCount--;
                    excess--;
                }
            }
        }

        private LogMessage FindLastActiveMessage()
        {
            for (int i = vrConsoleLogger.LogMessageParent.childCount - 1; i >= 0; i--)
            {
                LogMessage potentialMessage = vrConsoleLogger.LogMessageParent.GetChild(i).GetComponent<LogMessage>();
                if (potentialMessage != null && potentialMessage.gameObject.activeSelf)
                {
                    return potentialMessage;
                }
            }
            return null;
        }

        private void AddMessagesToPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                LogMessage newMessage = Object.Instantiate(vrConsoleLogger.LogMessagePrefab, vrConsoleLogger.LogMessageParent).GetComponent<LogMessage>();
                newMessage.gameObject.SetActive(false);
                _logMessagePool.Enqueue(newMessage);
            }
        }
    }
}