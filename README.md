# 🏠 Smart Home Automation System

This is my first backend project built using **ASP.NET Core** as part of the **Software Engineering** course. The system allows users to control smart home devices and integrates with a **machine learning model** to automate decisions based on user mood, time, and activity.

## 🚀 Features

- 🔌 **Device Control:** Turn devices on/off (e.g. TV, lights) via API or ML recommendations  
- 💡 **Smart Lighting:** Adjusts brightness based on context  
- 🎵 **Music Recommendation:** Suggests music depending on your mood  
- 📅 **Time & Location Awareness:** System behavior changes based on time of day and user's location  
- 🎉 **Special Days Detection:** Handles actions on weekends, holidays, etc.

## 🤖 Machine Learning Integration

- The system connects with a **Python Flask ML model** hosted locally.  
- Based on input (mood, activity, etc.), the model returns predictions such as:
  - `"turn on tv"` → triggers the corresponding **Home Assistant** API  
  - `"turn off tv"` → calls the off command automatically

## 🔗 Home Assistant Integration

Uses Home Assistant's REST APIs to interact with real devices.  
Example endpoints:
```http
POST /api/services/homeassistant/turn_on  
POST /api/services/homeassistant/turn_off  
