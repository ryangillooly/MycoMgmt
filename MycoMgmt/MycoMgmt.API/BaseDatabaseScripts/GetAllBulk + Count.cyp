MATCH (x:Bulk)
                            OPTIONAL MATCH (x)<-[:HAS_PARENT]-(child)
                            WITH 
                                x,
                                collect(child.Name) as children,
                                count(x.Id) as entityCount
                            OPTIONAL MATCH (x)-[:INOCULATED_ON]->(iDay:Day)<-[:HAS_DAY]-(iMonth:Month)-[:HAS_MONTH]-(iYear:Year)
                            OPTIONAL MATCH (x)-[:CREATED_ON]   ->(cDay:Day)<-[:HAS_DAY]-(cMonth:Month)-[:HAS_MONTH]-(cYear:Year)
                            OPTIONAL MATCH (x)-[:MODIFIED_ON]  ->(mDay:Day)<-[:HAS_DAY]-(mMonth:Month)-[:HAS_MONTH]-(mYear:Year)
                            OPTIONAL MATCH (x)-[:FINISHED_ON]  ->(fDay:Day)<-[:HAS_DAY]-(fMonth:Month)-[:HAS_MONTH]-(fYear:Year)
                            OPTIONAL MATCH (cUser:User)-[:CREATED]   ->(x)
                            OPTIONAL MATCH (mUser:User)-[:MODIFIED]  ->(x)
                            OPTIONAL MATCH (iUser:User)-[:INOCULATED]->(x)
                            OPTIONAL MATCH (x)-[:HAS_STRAIN]->(strain:Strain)
                            OPTIONAL MATCH (x)-[:STORED_IN]->(location:Location)
                            OPTIONAL MATCH (x)-[:HAS_PARENT]->(parent)
                            OPTIONAL MATCH (x)-[:CREATED_USING]->(recipe:Recipe)
                            WITH
                                x, 
                                reduce(x = '', i IN children | x + i + ',') as childrenString,
                                datetime({{year: iYear.year, month: iMonth.month, day: iDay.day}}) as inoculatedDate,
                                datetime({{year: cYear.year, month: cMonth.month, day: cDay.day}}) as createdDate,
                                datetime({{year: mYear.year, month: mMonth.month, day: mDay.day}}) as modifiedDate,
                                datetime({{year: fYear.year, month: fMonth.month, day: fDay.day}}) as finishedDate,
                                properties(cUser).Name as createdBy,
                                properties(mUser).Name as modifiedBy,
                                properties(iUser) as inoculatedBy,
                                properties(strain).Name as strain,
                                properties(location).Name as location,
                                parent,
                                sum(entityCount) as entityCount
                            WITH    
                                x,
                                left(childrenString, size(childrenString)-1) as children,
                                inoculatedDate,
                                createdDate,
                                modifiedDate,
                                finishedDate,
                                createdBy,
                                modifiedBy,
                                inoculatedBy,
                                strain,
                                location,
                                parent,
                                entityCount
                            WITH
                                entityCount,
                                apoc.map.mergeList
                                ([
                                    {{Id:    x.Id}},
                                    {{Name:         properties(x).Name}},
                                    {{EntityType:   properties(x).EntityType}},
                                    {{Notes:        properties(x).Notes}},
                                    {{Strain:       strain}},
                                    {{Status:       properties(x).Status}},
                                    {{Location:     location}},
                                    {{Parent:       properties(parent).Name}},
                                    {{ParentType:   properties(parent).EntityType}},
                                    {{Children:     children}},
                                    {{InoculatedOn: apoc.date.toISO8601(inoculatedDate.epochMillis, 'ms')}},
                                    {{InoculatedBy: inoculatedBy.Name}},
                                    {{CreatedOn:    apoc.date.toISO8601(createdDate.epochMillis, 'ms')}},
                                    {{CreatedBy:    createdBy}},
                                    {{ModifiedOn:   apoc.date.toISO8601(modifiedDate.epochMillis, 'ms')}},        
                                    {{ModifiedBy:   modifiedBy}},
                                    {{FinishedOn:   apoc.date.toISO8601(finishedDate.epochMillis, 'ms')}}
                                ])
                                as data
                            ORDER BY
                                finishedDate.day   DESC,
                                finishedDate.month DESC,
                                finishedDate.year  DESC,
                                properties(x).Name ASC
                            RETURN
                                apoc.map.mergeList
                                ([
                                    {{EntityCount: entityCount}},
                                    {{Data: data}}
                                ])
                                as result
                            SKIP 
                                0
                            LIMIT
                                20