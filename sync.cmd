@echo off

set repository=ssh://hg@bitbucket.org/IsaacRodriguez/civilizeddevelopment

hg pull %repository%
hg update