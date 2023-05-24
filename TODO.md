### Roodmap

* Docs:
    - [ ] Improve README.md. Add performance test result, update current states, add roadmap.
    - [ ] Generate and publish documentation using docfx.
    - [ ] Improve comments. Correct typos and update comments to reflect dotnet specifics.

* Tools:
    - [ ] Add a Help tool.
      Should provide additional help information about arguments and available formats.
    - [ ] Add IPF support for symbols.
      Each passed symbol can be represented in IPF format: "ipf[https://demo:demo@tools.dxfeed.com/ipf?]"
    - [ ] Add self-mode for PerfTest tool (produced and consume events and calc performance).
      Сan be used by the client to test performance their system.
    - [ ] Add SelfTest tool. Сan be used by the client to test their system.
      Can be very useful for a self-contained app to check the work on different platforms
      (checking for required lib, libpthread, vc_redist, etc.), must be run in docker on build.
      Сan be used by the client to test their system.
    - [ ] Add Multiplexor tool. Used for debugging. Can be specified uplink address for incoming data
      and downlink address for incoming subscription.

* API:
    - [ ] Add parser .properties file.
    - [ ] Add support xlm and json format for properties file.
    - [ ] Parsing and applying the default property file at the dotnet level, not java.
