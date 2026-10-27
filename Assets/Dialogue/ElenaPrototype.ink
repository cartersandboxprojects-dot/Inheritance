EXTERNAL elena_comforted()
EXTERNAL comfort_elena()
EXTERNAL charlie_met()
EXTERNAL meet_charlie()
EXTERNAL victoria_family()
EXTERNAL victoria_business()

-> elena

=== elena ===
{elena_comforted():
Elena: Thank you for staying with me earlier.
-> END
}
{charlie_met():
Elena: You spent the evening with Charlie. We can talk later.
-> END
}
Elena: It is hard to be here tonight.
* [Stay with Elena. Choose family.]
    ~ comfort_elena()
    {elena_comforted():
    Elena: Thank you. I do not feel quite so alone.
    - else:
    Elena: We should talk another time.
    }
    -> END
* [Leave for now.]
    -> END

=== charlie ===
{charlie_met():
Charlie: Thank you for discussing the business tonight.
-> END
}
{elena_comforted():
Charlie: The business discussion will have to wait.
-> END
}
Charlie: We need to talk about the business tonight.
* [Meet with Charlie. Choose business.]
    ~ meet_charlie()
    {charlie_met():
    Charlie: Good. Let us get to work.
    - else:
    Charlie: That opportunity has passed.
    }
    -> END
* [Leave for now.]
    -> END

=== victoria ===
{victoria_family():
Victoria: I see you have chosen family over business.
-> END
}
{victoria_business():
Victoria: Your mind is on business instead of family.
-> END
}
Victoria: I have not heard what you decided tonight.
-> END
