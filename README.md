# RadminVPNMetricDisablerPatch

### EDIT: Alternative workaround found!
Simply create a Registry entry (DWORD (32-bit)) named "AdjustMetric", with Value = 0, in:
HKEY_LOCAL_MACHINE \ SOFTWARE \ Famatech \ RadminVPN \ 1.0 (for x86), Or:
HKEY_LOCAL_MACHINE \ SOFTWARE \ WOW6432Node \ Famatech \ RadminVPN \ 1.0 (for x64)

## General Information
Radmin VPN is a VPN service, which can also be used for free. The software is interesting because it makes it relatively easy to set up a VPN where you can play LAN games over the Internet amongst other things. Users can be invited to separate groups, which can also be private (password protected). The biggest disadvantage, however, is that all hosts are located in a large subnet with mask 255.0.0.0. This means that 16,777,214 hosts can be reached via broadcast or single IP query. Thus, games that use this method to find matches in the same network are practically unusable, as the sheer number of hosts can hardly be scanned. On the other hand, games with direct IP input can be played very well with it.

The software is available at https://www.radmin-vpn.com/

## Network Card Metric
On Windows, the so-called metric defines the priority, in which network cards are used. Usually it is reasonable to place the LAN card first, and then Wifi, as the LAN cards usually have more bandwidth and a more stable connection.

So if a game only uses the first network card, it uses the network card with the lowest metric value set for any network card's TCP/IP protocol configuration of your system. You may set this manually at any time. Radmin VPN also adds a new network card, which serves to direct traffic through the VPN. It is of course also part of this hierarchy. However, it sets this network cards metric to 1 after every system start, which usually makes it first network card, even before your local network.

In many cases this can lead to problematic behavior, such as games all of a sudden not finding any servers hosted in the own LAN or local computers not being found for SMB access automatically any more. In order to omit this, it may help to reduce the metric to a lower priority/higher value. As this will be reset after a reboot automatically to value 1, it usually is sufficient to set the value to something like 20, to get access to the local network again and restart the system, once this is not desired any more.

Some users have requested that the Famatech (the developers of Radmin VPN) provide an option to toggle on/off this automatic metric in the application's settings, or as a configurable registry entry - however the developers have yet to implemented such functionality - for now it remains on the "wishlist" only (see https://radmin-club.com/radmin-vpn/radmin-vpn-interface-metric-automatic-change/ for more information).

## Radmin VPN Metric Disabler Patch
Radmin VPN Control Service (`RvControlSvc.exe`)  is responsible for automatically resetting the metric to 1 for Radmin VPN interface each time the service is reset (eg, on each reboot).

This patch will modify that file slightly so that it no longer forces the metric any longer.

You can then set the interface metric value to your own preference, or adjust the "Automatic metric" checkbox accordingly.

*Note 1:* The patch will only prevent Radmin VPN from resetting the metric each time it is restarted. You will still need to change or unset the existing metric value set for the Radmin VPN interface manually after applying the patch for the first time.

*Note 2:* Radmin VPN updates itself automatically whenever a new version is released, so it is likely that you may need to re-apply the patch if a new release causes the `RvControlSvc.exe` file to be replaced.

## How to apply the patch
*Step 1 (optional):* Backup your `RvControlSvc.exe`, as the patcher won't create any backup for you

*Step 2:* Download the `RadminVPNMetricDisablerPatch.exe` executable from the releases section of this repo to destination of your choice

*Step 3:* Stop Radmin VPN Control Service

*Step 4:* Run `RadminVPNMetricDisablerPatch.exe` with Administrator rights, and read console output to confirm that the operation was successful.

*Step 5:* Start Radmin VPN Control Service, or reboot if you prefer

### Note
- This patch requires .Net Framework v4.5.2 or higher, to function

- Patcher will look for `RvControlSvc.exe` first in `C:\Program Files\Radmin VPN\`, and then `C:\Program Files (x86)\Radmin VPN\` only, before giving up

## Disclaimer
Altering Radmin VPN's behaviour in this way is not officially supported right now, and may lead to undesirable effects, eg certain multiplayer games (which depend on metric being 1) no longer working as expected. So if something is not working for you after you applied this patch, please be sure to reinstall original software from https://www.radmin-vpn.com/ and test again, before creating any new support tickets at Radmin support forums/club.

### Donate
Did this make you happy? Please donate to show your support :)

BTC: 1Q4QkBn2Rx4hxFBgHEwRJXYHJjtfusnYfy

XMR: 4AfeGxGR4JqDxwVGWPTZHtX5QnQ3dTzwzMWLBFvysa6FTpTbz8Juqs25XuysVfowQoSYGdMESqnvrEQ969nR9Q7mEgpA5Zm
