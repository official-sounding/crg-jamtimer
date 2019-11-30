# CRG Jam Timer

this is a standalone application that can listen to a CRG Scoreboard's Websocket connection and play sounds when certain events happen on the scoreboard.  It is intented to be an "Automatic Jam Timer" that allows a scoreboard operator to work without a Jam Timer (and without having to have a whistle in their mouth).

It is a C# .NET Core 3.0 application.  It is licensed under the MIT License.  As NAudio only supports Windows, the program only works on Windows.

# Current functionality

* Play Single Whistle on Jam Start
* Play Four Whistles at Jam End/Timeout Start
* Rolling Whistle at Timeout End
* Can point at remote CRG running on a different host
* Can run in "Silent" mode that only reports events to the console


# To-Do

* Unit Tests
* Add a "Five Seconds" audio cue
* Find replacement for NAudio that supports OSX/Linux
* Have "better" Jam End whistle (Three sets of four whistles, only triggered at Jam conclusion)
* Build small GUI instead of Console app
* Add button to manually trigger Five Seconds (f.ex if coming out of timeout)
* Trigger Jam Start automatically five seconds after Jam Start.