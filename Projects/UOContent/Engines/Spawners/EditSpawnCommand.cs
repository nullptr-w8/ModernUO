/*************************************************************************
 * ModernUO                                                              *
 * Copyright (C) 2019-2021 - ModernUO Development Team                   *
 * Email: hi@modernuo.com                                                *
 * File: EditSpawnCommand.cs                                             *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Commands.Generic;
using Server.Network;

namespace Server.Engines.Spawners
{
    public class EditSpawnCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register(new EditSpawnCommand());
        }

        public EditSpawnCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Complex | CommandSupport.Simple;
            Commands = new[] { "EditSpawner" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "EditSpawner <mobile> <arguments> set <properties>";
            Description = "Modifies the given ";
            ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, List<object> list)
        {
            var args = e.Arguments;

            if (args.Length <= 1)
            {
                LogFailure(Usage);
                return;
            }

            if (list.Count == 0)
            {
                LogFailure("No matching objects found.");
                return;
            }

            var name = args[0];

            var type = AssemblyHandler.FindTypeByName(name);

            if (!Add.IsEntity(type))
            {
                LogFailure("No type with that name was found.");
                return;
            }

            var argSpan = e.ArgString.AsSpan(name.Length + 1);
            var setIndex = argSpan.InsensitiveIndexOf("set ");

            ReadOnlySpan<char> props = null;

            if (setIndex > -1)
            {
                var start = setIndex + 4;
                props = argSpan.Slice(start, argSpan.Length - start);
                argSpan = argSpan.SliceToLength(setIndex);
            }

            var argStr = argSpan.ToString().DefaultIfNullOrEmpty(null);
            var propsStr = props.ToString().DefaultIfNullOrEmpty(null);

            foreach (var obj in list)
            {
                if (obj is BaseSpawner spawner)
                {
                    UpdateSpawner(spawner, name, argStr, propsStr);
                }
            }
        }

        public static void UpdateSpawner(BaseSpawner spawner, string name, string arguments, string properties)
        {
            foreach (var entry in spawner.Entries)
            {
                // TODO: Should cache spawn type on the entry
                if (entry.SpawnedName.InsensitiveEquals(name))
                {
                    if (arguments != null)
                    {
                        entry.Parameters = arguments;
                    }

                    entry.Properties = properties;
                }
            }
        }
    }
}
